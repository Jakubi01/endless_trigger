using System.Linq.Expressions;
using Character;
using Character.Enemy;
using Items.Projectile;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(ProjectilePoolManager))]
    public abstract class WeaponAttackBase : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private float baseFireInterval = 3f;
        [SerializeField] private float damage;
        [SerializeField] private float cooldown;

        private float _attackSpeedMultiplier = 1f;
        private float _timer;
        private bool _isFiringEnabled;

        protected EnemyCharacterBase CurrentTarget;
        protected ProjectilePoolManager ProjectileManager;
        public CharacterBase owner;

        protected float Damage => damage;
        public float CurrentFireInterval => Mathf.Max(0.1f, (baseFireInterval + cooldown) / _attackSpeedMultiplier);
        public float RemainingCooldown => _isFiringEnabled ? Mathf.Max(0f, CurrentFireInterval - _timer) : 0f;
        public float CooldownProgress => CurrentFireInterval <= 0f ? 0f : Mathf.Clamp01(RemainingCooldown / CurrentFireInterval);

        protected virtual void Awake()
        {
            ProjectileManager = GetComponent<ProjectilePoolManager>();
            if (damage <= 0f)
            {
                damage = GetDefaultDamage();
            }
            
            owner = GetComponentInParent<CharacterBase>();
        }

        public void StartFiring()
        {
            if (_isFiringEnabled) return;

            _isFiringEnabled = true;
            _timer = CurrentFireInterval;
        }

        public void StopFiring()
        {
            _isFiringEnabled = false;
        }

        protected virtual void Update()
        {
            if (!_isFiringEnabled) return;

            _timer += Time.deltaTime;
            
            if (_timer >= CurrentFireInterval)
            {
                UpdateTarget();
                if (!CurrentTarget) return;
                
                _timer = 0f;
                Attack();
            }
        }

        protected abstract void Attack();
        protected abstract void UpdateTarget();

        public virtual void ModifyAttackSpeed(float newMultiplier)
        {
            _attackSpeedMultiplier = Mathf.Max(0.01f, newMultiplier);
        }

        public virtual void ApplyRandomUpgrade()
        {
        }

        protected void AddDamage(float amount)
        {
            damage = Mathf.Max(0f, damage + amount);
        }

        protected void AddCooldown(float amount)
        {
            cooldown += amount;
        }

        protected virtual float GetDefaultDamage()
        {
            return 10f;
        }
    }
}
