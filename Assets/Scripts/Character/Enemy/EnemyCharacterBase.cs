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
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private float contactDamageInterval = 1f;
        [SerializeField] private float proximityLimitRadius = 1f;
        
        protected HealthComponent EnemyHealthComponent;

        public float AttackRadius => attackRadius;

        private EnemyType _enemyType;
        private EnemyManager _enemyManager;
        private Action<EnemyCharacterBase> _releaseToPool;
        private BoxCollider2D _hitAreaBoxCollider2D;
        private CircleCollider2D _proximityLimitCircleCollider2D;
        private WaitForSeconds _waitForDiedAnimationEnd;
        private int _experienceReward;
        private float _contactDamage;
        private float _nextContactDamageTime;
        
        // Register attack motions and logic for all enemy units in this event
        public event Action OnAttackAction;

        // It called when character death
        public event Action OnDeathAction;
        
        protected PlayerCharacter Target { get; private set; }

        public bool IsAlive => EnemyHealthComponent && EnemyHealthComponent.IsAlive;

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
            
            const string clipName = "Death";
            var length = AnimationExtension.GetAnimClipLength(animator, clipName);

            if (length <= 0f)
            {
                length = 1.5f;
            }

            _waitForDiedAnimationEnd = new WaitForSeconds(length);
        }
        
        private void OnEnable()
        {
            OnAttackAction += Attack;
            OnDeathAction += Death;
            EnemyHealthComponent?.ResetHealth();
            _nextContactDamageTime = 0f;
            _enemyManager ??= EnemyManager.Instance;
            _enemyManager?.Register(this);
        }

        private void OnDestroy()
        {
            if (EnemyHealthComponent)
            {
                EnemyHealthComponent.Died -= OnDeath;
            }
        }

        private void OnDisable()
        {
            OnAttackAction -= Attack;
            OnDeathAction -= Death;
            _enemyManager?.Unregister(this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerCharacter player)) return;
            
            TryDamagePlayer(player);
        }

        public void Initialize(EnemyType type, float hp, float moveSpeed, float damage, int expReward)
        {
            _enemyType = type;
            _contactDamage = damage;
            _experienceReward = expReward;
            SetMoveSpeed(moveSpeed);
            EnemyHealthComponent.Initialize(hp);
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

        public virtual void TakeDamage(float amount)
        {
            if(damageSound)
                soundManager.PlaySfx(damageSound);
            
            EnemyHealthComponent?.TakeDamage(amount);
            animator.SetTrigger(AnimatorParamToHash.TakeDamage);
            DamageTextManager.Instance?.ShowDamageText(transform.position, amount.ToString(CultureInfo.InvariantCulture));
        }

        private void InitializeHealthComponent()
        {
            EnemyHealthComponent = GetComponent<HealthComponent>();
            if (!EnemyHealthComponent)
            {
                EnemyHealthComponent = gameObject.AddComponent<HealthComponent>();
            }

            EnemyHealthComponent.Died += OnDeath;
        }

        private void TryDamagePlayer(PlayerCharacter playerCharacter)
        {
            if (Time.time < _nextContactDamageTime) return;
            
            Target = playerCharacter;
            
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack")) return;
            
            // 애니메이션 클립에 Animation Event에 등록된 DamageToTarget이 실행되면서
            // OnAttackAction에 등록된 함수들이 실행됨
            OnAttackAction?.Invoke();
        }
        
        // Animation Event 구독용
        protected void DamageToTarget()
        {
            if(damageSound)
                soundManager.PlaySfx(damageSound);
            
            Target?.TakeDamage(_contactDamage);
            _nextContactDamageTime = Time.time + contactDamageInterval;
        }

        protected virtual void Attack()
        {
            animator.SetTrigger(AnimatorParamToHash.EnemyAttack);
        }

        protected virtual void Death()
        {
            if(deathSound)
                soundManager.PlaySfx(deathSound);
                
            animator.SetTrigger(AnimatorParamToHash.Died);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            
            OnDeathAction?.Invoke();
            
            GameManager.Instance?.RegisterKill();
            _enemyManager?.SpawnExperience(transform.position, _experienceReward);

            StartCoroutine(WaitForDiedAnimationEnd());
        }

        private IEnumerator WaitForDiedAnimationEnd()
        {
            yield return _waitForDiedAnimationEnd;
            
            if (_releaseToPool != null)
            {
                _releaseToPool.Invoke(this);
                yield break;
            }

            Destroy(gameObject);
        }

        private void SetProximityLimitRadius(float radius)
        {
            _proximityLimitCircleCollider2D.radius = radius;
        }

        public void StartBuffCoroutine(IEnumerator applySpeedBuff)
        {
            StartCoroutine(applySpeedBuff);
        }
    }
}
