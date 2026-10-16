using Items.ProjectileManager;
using UnityEngine;

namespace Character.Enemy
{
    [RequireComponent(typeof(ProjectilePoolManager))]
    public class GunnerCharacter : EnemyCharacterBase
    {
        // 플레이어 방향으로 3초마다 60° 범위 투사체 3발 동시 발사, 사거리 8m
        // contactDamageInterval = 3f;
        [Header("Attack Settings")] 
        [SerializeField] private float spreadAngle = 60f;
        [SerializeField] private int projectileCount = 3;
        [SerializeField] private float projectileDamage = 10f;
        
        private ProjectilePoolManager _projectilePoolManager;
        
        protected override void Awake()
        {
            base.Awake();
            
            _projectilePoolManager = GetComponent<ProjectilePoolManager>();
        }

        private void OnEnable()
        {
            OnAttackAction += OnAttack;
            OnDeathAction += OnDeath;
        }

        private void OnDisable()
        {
            OnAttackAction -= OnAttack;
            OnDeathAction -= OnDeath;
        }

        private void OnAttack()
        {
            Vector2 directionToPlayer = (Target.transform.position - transform.position).normalized;
            var baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            
            var halfSpread = spreadAngle / 2f;
            var angleStep = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;

            for (var i = 0; i < projectileCount; i++)
            {
                var currentAngle = baseAngle - halfSpread + (angleStep * i);
                var rotation = Quaternion.Euler(0, 0, currentAngle);
                
                var fireDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

                var projectile = _projectilePoolManager.Spawn(
                    position: transform.position,
                    rotation: rotation,
                    direction: fireDirection,
                    damage: projectileDamage
                );

                if (!projectile.TryGetComponent(out Projectile projectileInstance)) continue;

                projectileInstance.SetLifeTime(3f);
            }
        }

        protected override void OnDeath()
        {
            
        }
    }
}