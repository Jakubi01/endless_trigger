using Character.Player;
using Components;
using System;
using System.Collections;
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
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private int experienceReward = 5;
        [SerializeField] private float contactDamageInterval = 1f;
        [SerializeField] private float proximityLimitRadius = 1f;

        private EnemyManager _enemyManager;
        private float _nextContactDamageTime;
        private HealthComponent _healthComponent;
        private Action<EnemyCharacterBase> _releaseToPool;
        private BoxCollider2D _hitAreaBoxCollider2D;
        private CircleCollider2D _proximityLimitCircleCollider2D;

        // 현재 구조는 AttackAction에 event를 구독시켜 Animation을 재생하는 방식인데,
        // 더 나은 방법은 Animation Event에 public void Foo()를 등록해서 
        // TryDamagePlayer => PlayAttackAnim() 실행
        // Foo => 데미지 처리
        // 이 구조가 훨씬 깔끔함
        // 대신 이 프로젝트에는 각 유닛마다 고유의 공격 애니메이션이 없음..
        // -> 공격 애니메이션을 기획이나 아트에서 만들어줘야 하는데 . . .
        
        // Register attack motions and logic for all enemy units in this event
        public event Action OnAttackAction;

        // It called when character death
        public event Action OnDeathAction;
        
        protected PlayerCharacter Target { get; private set; }

        public bool IsAlive => _healthComponent && _healthComponent.IsAlive;

        protected override void Awake()
        {
            base.Awake();
            
            InitializeHealthComponent();
            
            _enemyManager = EnemyManager.Instance;
            
            GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
            
            _hitAreaBoxCollider2D = GetComponent<BoxCollider2D>();
            _hitAreaBoxCollider2D.size = new Vector2(attackRadius, attackRadius);
            
            _proximityLimitCircleCollider2D = GetComponent<CircleCollider2D>();
            SetProximityLimitRadius(proximityLimitRadius);
        }

        // TODO: OnEnable, OnDestroy, OnDisable protected | virtual로 만들기
        
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
            if (!other.TryGetComponent(out PlayerCharacter player)) return;
            
            TryDamagePlayer(player);
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

        protected void TryDamagePlayer(PlayerCharacter playerCharacter)
        {
            if (Time.time < _nextContactDamageTime) return;
            
            Target = playerCharacter;
            
            OnAttackAction?.Invoke();
            
            // Animation Event를 사용하면 아래의 로직은 삭제
            playerCharacter.TakeDamage(contactDamage);
            _nextContactDamageTime = Time.time + contactDamageInterval;
        }

        // Animation Event 구독용
        public void DamageToTarget()
        {
            Target.TakeDamage(contactDamage);
            _nextContactDamageTime = Time.time + contactDamageInterval;
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            
            OnDeathAction?.Invoke();
            
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

        public void StartBuffCoroutine(IEnumerator applySpeedBuff)
        {
            StartCoroutine(applySpeedBuff);
        }
    }
}
