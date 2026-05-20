using System;
using Character.Enemy;
using Character.Player;
using Items.Projectile;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(ProjectilePoolManager))]
    public abstract class WeaponBase : MonoBehaviour
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
        public PlayerCharacter owner;

        public float CurrentFireInterval => Mathf.Max(0.01f, baseFireInterval / _attackSpeedMultiplier);

        protected virtual void Awake()
        {
            ProjectileManager = GetComponent<ProjectilePoolManager>();
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
            LookAtTarget();

            while (_timer >= CurrentFireInterval)
            {
                _timer -= CurrentFireInterval;
                Fire();
            }
        }

        protected abstract void Fire();
        protected abstract void LookAtTarget();

        public virtual void ModifyAttackSpeed(float newMultiplier)
        {
            _attackSpeedMultiplier = Mathf.Max(0.01f, newMultiplier);
        }
    }
}