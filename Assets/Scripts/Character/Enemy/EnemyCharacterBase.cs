using Character.Player;
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

    public class EnemyCharacterBase : CharacterBase, IDamageable
    {
        [Header("Enemy Status")]
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private float maxHp = 30f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private int experienceReward = 5;
        [SerializeField] private float contactDamageInterval = 0.5f;

        private EnemyManager _enemyManager;
        private float _currentHp;
        private float _contactDamageTimer;

        public EnemyType EnemyType => enemyType;
        public bool IsAlive => _currentHp > 0f;

        protected override void Awake()
        {
            base.Awake();
            Rb.bodyType = RigidbodyType2D.Kinematic;
            Rb.simulated = true;
            _enemyManager = EnemyManager.Instance;
        }

        private void OnEnable()
        {
            _currentHp = maxHp;
            _contactDamageTimer = 0f;
            _enemyManager ??= EnemyManager.Instance;
            _enemyManager?.Register(this);
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

        private void Update()
        {
            if (_contactDamageTimer > 0f)
            {
                _contactDamageTimer -= Time.deltaTime;
            }
        }

        public void Initialize(EnemyType type, float hp, float moveSpeed, float damage, int expReward)
        {
            enemyType = type;
            maxHp = hp;
            contactDamage = damage;
            experienceReward = expReward;
            SetMoveSpeed(moveSpeed);
            _currentHp = maxHp;
        }

        public void MoveToTargetPosition(Vector2 targetPosition)
        {
            if (_currentHp <= 0f) return;

            MoveToward(targetPosition);
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || _currentHp <= 0f) return;

            _currentHp -= amount;
            if (_currentHp <= 0f)
            {
                Die();
            }
        }

        private void TryDamagePlayer(Collider2D other)
        {
            if (_contactDamageTimer > 0f) return;

            if (other.TryGetComponent(out PlayerCharacter player))
            {
                player.TakeDamage(contactDamage);
                _contactDamageTimer = contactDamageInterval;
            }
        }

        private void Die()
        {
            _enemyManager?.SpawnExperience(transform.position, experienceReward);
            Destroy(gameObject);
        }
    }
}
