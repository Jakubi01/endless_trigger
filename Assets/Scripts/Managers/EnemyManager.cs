using System.Collections.Generic;
using Character.Enemy;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject experiencePickupPrefab;

        private readonly List<EnemyCharacterBase> _enemies = new();
        public static EnemyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            if (!GetComponent<EnemySpawner>())
            {
                gameObject.AddComponent<EnemySpawner>();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Register(EnemyCharacterBase enemy)
        {
            if (enemy && !_enemies.Contains(enemy))
            {
                IgnoreEnemyCollisions(enemy);
                _enemies.Add(enemy);
            }
        }

        public void Unregister(EnemyCharacterBase enemy)
        {
            _enemies.Remove(enemy);
        }

        public IReadOnlyList<EnemyCharacterBase> Enemies => _enemies;

        public GameObject FindNearestEnemy(Transform callerTransform, float range)
        {
            EnemyCharacterBase nearest = null;
            float nearestSqrDistance = range * range;

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                EnemyCharacterBase enemy = _enemies[i];
                if (!enemy)
                {
                    _enemies.RemoveAt(i);
                    continue;
                }

                float sqrDistance = (enemy.transform.position - callerTransform.position).sqrMagnitude;
                if (sqrDistance <= nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearest = enemy;
                }
            }

            return nearest ? nearest.gameObject : null;
        }

        private void IgnoreEnemyCollisions(EnemyCharacterBase newEnemy)
        {
            Collider2D newCollider = newEnemy.CharacterCollider;
            if (!newCollider) return;

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                EnemyCharacterBase enemy = _enemies[i];
                if (!enemy)
                {
                    _enemies.RemoveAt(i);
                    continue;
                }

                Collider2D enemyCollider = enemy.CharacterCollider;
                if (enemyCollider)
                {
                    Physics2D.IgnoreCollision(newCollider, enemyCollider, true);
                }
            }
        }

        public GameObject FindFarthestEnemy(Transform callerTransform, float range)
        {
            EnemyCharacterBase farthest = null;
            float farthestSqrDistance = -1f;
            float maxSqrDistance = range * range;

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                EnemyCharacterBase enemy = _enemies[i];
                if (!enemy)
                {
                    _enemies.RemoveAt(i);
                    continue;
                }

                float sqrDistance = (enemy.transform.position - callerTransform.position).sqrMagnitude;
                if (sqrDistance <= maxSqrDistance && sqrDistance > farthestSqrDistance)
                {
                    farthestSqrDistance = sqrDistance;
                    farthest = enemy;
                }
            }

            return farthest ? farthest.gameObject : null;
        }

        public void SpawnExperience(Vector3 position, int amount)
        {
            if (amount <= 0) return;

            if (experiencePickupPrefab)
            {
                GameObject pickup = Instantiate(experiencePickupPrefab, position, Quaternion.identity);
                if (pickup.TryGetComponent(out ExperiencePickup experiencePickup))
                {
                    experiencePickup.Initialize(amount);
                }

                return;
            }

            // fallback object
            GameObject fallback = new GameObject("ExperiencePickup");
            fallback.transform.position = position;

            CircleCollider2D collider = fallback.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.2f;

            SpriteRenderer renderer = fallback.AddComponent<SpriteRenderer>();
            renderer.color = Color.cyan;

            fallback.AddComponent<ExperiencePickup>().Initialize(amount);
        }
    }
}
