using System;
using Character.Enemy;
using Character.Player;
using Controllers.Enemy;
using UnityEngine;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        private struct EnemyPrefabEntry
        {
            public EnemyType type;
            public GameObject prefab;
        }

        [Serializable]
        private struct SpawnRate
        {
            public int spawnCount;
            public float zombieWeight;
            public float rusherWeight;
            public float tankerWeight;
        }

        [SerializeField] private EnemyPrefabEntry[] enemyPrefabs;
        [SerializeField] private float minSpawnDistanceFromPlayer = 5f;
        [SerializeField] private float maxSpawnDistanceFromPlayer = 9f;
        [SerializeField] private float waveDuration = 30f;
        [SerializeField] private float statGrowthPerWave = 0.15f;
        [SerializeField] private int baseSpawnCount = 2;
        [SerializeField] private int spawnCountIncreasePerSection = 2;
        [SerializeField] private int spawnCountIncreasePerWave = 1;
        [SerializeField] private float minimumZombieRatio = 0.05f;
        [SerializeField] private float maximumTankerRatio = 0.75f;
        [SerializeField] private int enemySortingOrder = 10;

        private const float SpawnInterval = 5f;

        private PlayerCharacter _player;
        private float _elapsedTime;
        private float _nextSpawnTime;

        private void Awake()
        {
            if (maxSpawnDistanceFromPlayer < minSpawnDistanceFromPlayer)
            {
                maxSpawnDistanceFromPlayer = minSpawnDistanceFromPlayer;
            }
        }

        private void Update()
        {
            if (!_player)
            {
                _player = FindFirstObjectByType<PlayerCharacter>();
                if (!_player) return;
            }

            _elapsedTime += Time.deltaTime;

            while (_elapsedTime >= _nextSpawnTime)
            {
                SpawnWaveSection(_nextSpawnTime);
                _nextSpawnTime += SpawnInterval;
            }
        }

        private void SpawnWaveSection(float spawnTime)
        {
            SpawnRate rate = GetSpawnRate(spawnTime);
            for (int i = 0; i < rate.spawnCount; i++)
            {
                SpawnEnemy(PickEnemyType(rate), spawnTime);
            }
        }

        private SpawnRate GetSpawnRate(float spawnTime)
        {
            int waveNumber = Mathf.FloorToInt(spawnTime / waveDuration) + 1;
            float timeInWave = spawnTime % waveDuration;
            int sectionIndex = Mathf.Clamp(Mathf.FloorToInt(timeInWave / SpawnInterval), 0, 5);
            float sectionProgress = sectionIndex / 5f;
            float difficulty = waveNumber - 1 + sectionProgress;

            float zombieRatio = Mathf.Clamp(1f - difficulty * 0.18f, minimumZombieRatio, 1f);
            float tankerRatio = Mathf.Clamp((difficulty - 1f) * 0.1f, 0f, maximumTankerRatio);
            float rusherRatio = Mathf.Max(0f, 1f - zombieRatio - tankerRatio);

            if (zombieRatio + rusherRatio + tankerRatio > 1f)
            {
                float overflow = zombieRatio + rusherRatio + tankerRatio - 1f;
                zombieRatio = Mathf.Max(minimumZombieRatio, zombieRatio - overflow);
            }

            return new SpawnRate
            {
                spawnCount = GetSpawnCount(waveNumber, sectionIndex),
                zombieWeight = zombieRatio,
                rusherWeight = rusherRatio,
                tankerWeight = tankerRatio
            };
        }

        private EnemyType PickEnemyType(SpawnRate rate)
        {
            float totalWeight = rate.zombieWeight + rate.rusherWeight + rate.tankerWeight;
            if (totalWeight <= 0f) return EnemyType.Zombie;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            if (roll < rate.zombieWeight) return EnemyType.Zombie;

            roll -= rate.zombieWeight;
            return roll < rate.rusherWeight ? EnemyType.Rusher : EnemyType.Tanker;
        }

        private int GetSpawnCount(int waveNumber, int sectionIndex)
        {
            int waveBonus = Mathf.Max(0, waveNumber - 1) * spawnCountIncreasePerWave;
            int sectionBonus = sectionIndex * spawnCountIncreasePerSection;
            return Mathf.Max(1, baseSpawnCount + waveBonus + sectionBonus);
        }

        private void SpawnEnemy(EnemyType type, float spawnTime)
        {
            GameObject prefab = FindPrefab(type);
            if (!prefab)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: {type} prefab is not assigned.", this);
                return;
            }

            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude < 0.01f)
            {
                direction = Vector2.right;
            }

            float distance = UnityEngine.Random.Range(minSpawnDistanceFromPlayer, maxSpawnDistanceFromPlayer);
            Vector3 spawnPosition = _player.transform.position + (Vector3)(direction * distance);
            GameObject enemyObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            if (!enemyObject.TryGetComponent(out EnemyCharacterBase enemy))
            {
                enemy = AddEnemyCharacter(enemyObject, type);
            }

            GetBaseStats(type, out float hp, out float speed, out float damage, out int experience);
            int waveNumber = Mathf.FloorToInt(spawnTime / waveDuration) + 1;
            float statMultiplier = 1f + Mathf.Max(0, waveNumber - 1) * statGrowthPerWave;
            enemy.Initialize(type, hp * statMultiplier, speed, damage * statMultiplier, experience);
            EnsureEnemyController(enemyObject, type);
            EnsureEnemyVisual(enemyObject, type);
        }

        private GameObject FindPrefab(EnemyType type)
        {
            foreach (EnemyPrefabEntry entry in enemyPrefabs)
            {
                if (entry.type == type && entry.prefab)
                {
                    return entry.prefab;
                }
            }

            return null;
        }

        private static EnemyCharacterBase AddEnemyCharacter(GameObject enemyObject, EnemyType type)
        {
            return type switch
            {
                EnemyType.Rusher => enemyObject.AddComponent<RusherCharacter>(),
                EnemyType.Tanker => enemyObject.AddComponent<TankerCharacter>(),
                _ => enemyObject.AddComponent<ZombieCharacter>()
            };
        }

        private static void EnsureEnemyController(GameObject enemyObject, EnemyType type)
        {
            if (enemyObject.GetComponent<EnemyControllerBase>()) return;

            switch (type)
            {
                case EnemyType.Rusher:
                    enemyObject.AddComponent<RusherController>();
                    break;
                case EnemyType.Tanker:
                    enemyObject.AddComponent<TankerController>();
                    break;
                default:
                    enemyObject.AddComponent<ZombieController>();
                    break;
            }
        }

        private void EnsureEnemyVisual(GameObject enemyObject, EnemyType type)
        {
            SpriteRenderer renderer = enemyObject.GetComponentInChildren<SpriteRenderer>();
            if (!renderer)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: {type} prefab has no SpriteRenderer.", enemyObject);
                return;
            }

            if (!renderer.sprite)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: {type} prefab has no sprite assigned.", enemyObject);
            }

            if (renderer.sortingOrder < enemySortingOrder)
            {
                renderer.sortingOrder = enemySortingOrder;
            }
        }

        private static void GetBaseStats(EnemyType type, out float hp, out float speed, out float damage, out int experience)
        {
            switch (type)
            {
                case EnemyType.Rusher:
                    hp = 15f;
                    speed = 6f;
                    damage = 15f;
                    experience = 8;
                    break;
                case EnemyType.Tanker:
                    hp = 30f;
                    speed = 2.2f;
                    damage = 25f;
                    experience = 20;
                    break;
                default:
                    hp = 30f;
                    speed = 3.2f;
                    damage = 10f;
                    experience = 5;
                    break;
            }
        }
    }
}
