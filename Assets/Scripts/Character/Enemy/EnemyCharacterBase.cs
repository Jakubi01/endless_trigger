using Character.Player;
using Components;
using System;
using Managers;
using UnityEngine;

namespace Character.Enemy
{
    public enum EnemyType
    {
        Zombie,
        Rusher,
        Tanker
    }

    [RequireComponent(typeof(HealthComponent))]
    public class EnemyCharacterBase : CharacterBase, IDamageable
    {
        [Header("Enemy Status")]
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private int experienceReward = 5;
        [SerializeField] private float contactDamageInterval = 0.5f;

        private EnemyManager _enemyManager;
        private float _nextContactDamageTime;
        private HealthComponent _healthComponent;
        private Action<EnemyCharacterBase> _releaseToPool;

        public EnemyType EnemyType => enemyType;
        public bool IsAlive => _healthComponent && _healthComponent.IsAlive;

        protected override void Awake()
        {
            base.Awake();
            InitializeHealthComponent();
            _enemyManager = EnemyManager.Instance;
            GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
        }

        private void OnEnable()
        {
            _healthComponent?.ResetHealth();
            _nextContactDamageTime = 0f;
            _enemyManager ??= EnemyManager.Instance;
            _enemyManager?.Register(this);
        }

        private void OnDestroy()
        {
            if (_healthComponent)
            {
                _healthComponent.Died -= Die;
            }
        }

        private void OnDisable()
        {
            _enemyManager?.Unregister(this);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryDamagePlayer(collision.collider);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamagePlayer(other);
        }

        public void Initialize(EnemyType type, float hp, float moveSpeed, float damage, int expReward)
        {
            enemyType = type;
            contactDamage = damage;
            experienceReward = expReward;
            SetMoveSpeed(moveSpeed);
            _healthComponent.Initialize(hp);
        }

        public void SetPoolReleaseAction(Action<EnemyCharacterBase> releaseAction)
        {
            _releaseToPool = releaseAction;
        }

        public void MoveToTargetPosition(Vector2 targetPosition)
        {
            if (!IsAlive) return;

            MoveToward(targetPosition);
        }

        public void TakeDamage(float amount)
        {
            _healthComponent?.TakeDamage(amount);
        }

        private void InitializeHealthComponent()
        {
            _healthComponent = GetComponent<HealthComponent>();
            if (!_healthComponent)
            {
                _healthComponent = gameObject.AddComponent<HealthComponent>();
            }

            _healthComponent.Died += Die;
        }

        private void TryDamagePlayer(Collider2D other)
        {
            if (Time.time < _nextContactDamageTime) return;

            if (other.TryGetComponent(out PlayerCharacter player))
            {
                player.TakeDamage(contactDamage);
                _nextContactDamageTime = Time.time + contactDamageInterval;
            }
        }

        private void Die()
        {
            GameManager.Instance?.RegisterKill();
            _enemyManager?.SpawnExperience(transform.position, experienceReward);
            if (_releaseToPool != null)
            {
                _releaseToPool.Invoke(this);
                return;
            }

            Destroy(gameObject);
        }
    }
}
