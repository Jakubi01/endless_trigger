using Character.Player;
using Components;
using System;
using System.Globalization;
using Effects.DamageText;
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
    [RequireComponent(typeof(CircleCollider2D))]
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
        private CircleCollider2D _proximityLimitCircleCollider2D;

        public EnemyType EnemyType => enemyType;
        public bool IsAlive => _healthComponent && _healthComponent.IsAlive;

        protected override void Awake()
        {
            base.Awake();
            InitializeHealthComponent();
            _enemyManager = EnemyManager.Instance;
            GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
            _proximityLimitCircleCollider2D = GetComponent<CircleCollider2D>();
            SetProximityLimitRadius(1f);
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
                _healthComponent.Died -= OnDeath;
            }
        }

        private void OnDisable()
        {
            _enemyManager?.Unregister(this);
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

        private void FixedUpdate()
        {
            if (!IsAlive) return;
            
            UpdateAnimation();
        }

        protected override bool UpdateAnimation()
        {
            var result = base.UpdateAnimation();
            animator.SetBool(AnimatorParamToHash.Move, result);
            return result;
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
            DamageTextManager.Instance?.ShowDamageText(transform.position, amount.ToString(CultureInfo.InvariantCulture));
        }

        private void InitializeHealthComponent()
        {
            _healthComponent = GetComponent<HealthComponent>();
            if (!_healthComponent)
            {
                _healthComponent = gameObject.AddComponent<HealthComponent>();
            }

            _healthComponent.Died += OnDeath;
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

        protected override void OnDeath()
        {
            base.OnDeath();
            
            GameManager.Instance?.RegisterKill();
            _enemyManager?.SpawnExperience(transform.position, experienceReward);
            if (_releaseToPool != null)
            {
                _releaseToPool.Invoke(this);
                return;
            }

            Destroy(gameObject);
        }
        
        protected void SetProximityLimitRadius(float radius)
        {
            _proximityLimitCircleCollider2D.radius = radius;
        }
    }
}
