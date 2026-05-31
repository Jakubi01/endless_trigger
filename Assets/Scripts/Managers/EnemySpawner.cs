using System.Collections.Generic;
using Character.Enemy;
using Character.Player;
using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        private struct SpawnRate
        {
            public int SpawnCount;
            public float ZombieWeight;
            public float RusherWeight;
            public float TankerWeight;
        }

        private sealed class EnemyPool
        {
            public EnemyData Data;
            public IObjectPool<EnemyCharacterBase> Pool;
        }

        [SerializeField] private EnemyData[] enemyData;
        [SerializeField] private float minSpawnDistanceFromPlayer = 5f;
        [SerializeField] private float maxSpawnDistanceFromPlayer = 9f;
        [SerializeField] private float initialSpawnDelay = 5f;
        [SerializeField] private float waveDuration = 30f;
        [SerializeField] private int baseSpawnCount = 2;
        [SerializeField] private int spawnCountIncreasePerSection = 2;
        [SerializeField] private int spawnCountIncreasePerWave = 1;
        [SerializeField] private float minimumZombieRatio = 0.05f;
        [SerializeField] private float maximumTankerRatio = 0.75f;
        [SerializeField] private int defaultPoolCapacity = 32;
        [SerializeField] private int maxPoolSize = 256;
        [SerializeField] private int prewarmCountPerEnemy = 8;

        private const float SpawnInterval = 5f;

        private readonly Dictionary<EnemyType, EnemyPool> _pools = new();
        private readonly List<EnemyCharacterBase> _prewarmBuffer = new();
        private PlayerCharacter _player;
        private float _elapsedTime;
        private float _nextSpawnTime;

        public float ElapsedTime => _elapsedTime;
        public float InitialSpawnDelay => initialSpawnDelay;
        public float WaveDuration => waveDuration;
        public float TimeUntilGameStart => Mathf.Max(0f, initialSpawnDelay - _elapsedTime);
        public float CurrentWaveRemainingTime
        {
            get
            {
                float activeTime = Mathf.Max(0f, _elapsedTime - initialSpawnDelay);
                float timeInWave = activeTime % waveDuration;
                return Mathf.Max(0f, waveDuration - timeInWave);
            }
        }

        private void Awake()
        {
            if (maxSpawnDistanceFromPlayer < minSpawnDistanceFromPlayer)
            {
                maxSpawnDistanceFromPlayer = minSpawnDistanceFromPlayer;
            }

            initialSpawnDelay = Mathf.Max(0f, initialSpawnDelay);
            waveDuration = Mathf.Max(0.1f, waveDuration);
            _nextSpawnTime = initialSpawnDelay;
            BuildPools();
        }

        private void OnDestroy()
        {
            foreach (EnemyPool enemyPool in _pools.Values)
            {
                enemyPool.Pool?.Clear();
            }

            _pools.Clear();
            _prewarmBuffer.Clear();
        }

        private void Update()
        {
            if (!_player)
            {
                _player = FindFirstObjectByType<PlayerCharacter>();
                if (!_player) return;
            }

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < _nextSpawnTime) return;

            float activeSpawnTime = Mathf.Max(0f, _nextSpawnTime - initialSpawnDelay);
            SpawnWaveSection(activeSpawnTime);
            _nextSpawnTime += SpawnInterval;
        }

        private void BuildPools()
        {
            _pools.Clear();
            if (enemyData == null || enemyData.Length == 0)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: EnemyData list is empty.", this);
                return;
            }

            foreach (EnemyData data in enemyData)
            {
                if (!data || !data.Prefab)
                {
                    Debug.LogError($"{nameof(EnemySpawner)}: EnemyData or prefab is not assigned.", this);
                    continue;
                }

                if (_pools.ContainsKey(data.EnemyType))
                {
                    Debug.LogError($"{nameof(EnemySpawner)}: Duplicate EnemyData for {data.EnemyType}.", this);
                    continue;
                }

                EnemyPool enemyPool = new EnemyPool { Data = data };
                enemyPool.Pool = new ObjectPool<EnemyCharacterBase>(
                    createFunc: () => CreateEnemy(enemyPool),
                    actionOnGet: enemy => enemy.gameObject.SetActive(true),
                    actionOnRelease: enemy => enemy.gameObject.SetActive(false),
                    actionOnDestroy: enemy => Destroy(enemy.gameObject),
                    collectionCheck: true,
                    defaultCapacity: defaultPoolCapacity,
                    maxSize: maxPoolSize
                );

                _pools.Add(data.EnemyType, enemyPool);
                Prewarm(enemyPool);
            }
        }

        private EnemyCharacterBase CreateEnemy(EnemyPool enemyPool)
        {
            EnemyCharacterBase enemy = Instantiate(enemyPool.Data.Prefab, transform);
            enemy.gameObject.SetActive(false);
            enemy.SetPoolReleaseAction(enemyPool.Pool.Release);
            return enemy;
        }

        private void Prewarm(EnemyPool enemyPool)
        {
            _prewarmBuffer.Clear();
            for (int i = 0; i < prewarmCountPerEnemy; i++)
            {
                _prewarmBuffer.Add(enemyPool.Pool.Get());
            }

            foreach (EnemyCharacterBase enemy in _prewarmBuffer)
            {
                enemyPool.Pool.Release(enemy);
            }
        }

        private void SpawnWaveSection(float spawnTime)
        {
            SpawnRate rate = GetSpawnRate(spawnTime);
            for (int i = 0; i < rate.SpawnCount; i++)
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
                SpawnCount = GetSpawnCount(waveNumber, sectionIndex),
                ZombieWeight = zombieRatio,
                RusherWeight = rusherRatio,
                TankerWeight = tankerRatio
            };
        }

        private EnemyType PickEnemyType(SpawnRate rate)
        {
            float totalWeight = rate.ZombieWeight + rate.RusherWeight + rate.TankerWeight;
            if (totalWeight <= 0f) return EnemyType.Zombie;

            float roll = Random.Range(0f, totalWeight);
            if (roll < rate.ZombieWeight) return EnemyType.Zombie;

            roll -= rate.ZombieWeight;
            return roll < rate.RusherWeight ? EnemyType.Rusher : EnemyType.Tanker;
        }

        private int GetSpawnCount(int waveNumber, int sectionIndex)
        {
            int waveBonus = Mathf.Max(0, waveNumber - 1) * spawnCountIncreasePerWave;
            int sectionBonus = sectionIndex * spawnCountIncreasePerSection;
            return Mathf.Max(1, baseSpawnCount + waveBonus + sectionBonus);
        }

        private void SpawnEnemy(EnemyType type, float spawnTime)
        {
            if (!_pools.TryGetValue(type, out EnemyPool enemyPool))
            {
                Debug.LogError($"{nameof(EnemySpawner)}: EnemyData for {type} is not assigned.", this);
                return;
            }

            EnemyCharacterBase enemy = enemyPool.Pool.Get();
            enemy.transform.position = GetSpawnPosition();
            enemy.transform.rotation = Quaternion.identity;

            int waveNumber = Mathf.FloorToInt(spawnTime / waveDuration) + 1;
            enemyPool.Data.GetScaledStats(waveNumber, out float hp, out float speed, out float damage, out int experience);
            enemy.Initialize(type, hp, speed, damage, experience);
        }

        private Vector3 GetSpawnPosition()
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude < 0.01f)
            {
                direction = Vector2.right;
            }

            float distance = Random.Range(minSpawnDistanceFromPlayer, maxSpawnDistanceFromPlayer);
            return _player.transform.position + (Vector3)(direction * distance);
        }
    }
}
