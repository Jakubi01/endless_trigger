using Character.Enemy;
using UnityEngine;
using Items.Projectile;

namespace Items.Weapon
{
    public class ShotGun : WeaponBase
    {
        [Header("ShotGun Settings")]
        [SerializeField] private int bulletCount = 5;
        [SerializeField] private float spreadAngle = 30;
        [SerializeField] private Transform muzzlePoint;
        private const float Range = 3f;
        
        protected override void Fire()
        {
            Vector3 fireDirection = transform.right;

            float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            float startAngle = baseAngle - (spreadAngle / 2f);
            float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;

            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                
                ProjectileManager.Spawn(muzzlePoint.position, rotation, dir);
            }
        }

        protected override void LookAtTarget()
        {
            var nearest = owner.FindNearestFromCharacter(Range);
            CurrentTarget = nearest ? nearest.GetComponent<EnemyCharacterBase>() : null;
            
            Vector3 targetDirection = owner.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            if (CurrentTarget)
            {
                targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
            }
            
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            
            if (owner.transform.localScale.x < 0)
            {
                // 플레이어가 왼쪽을 볼 때 부모 Scale.x가 -1이 되므로,
                // 무기의 로컬 회전각을 180도 반전시켜야 무기가 정상 조준됩니다.
                targetAngle += 180f;

                // 무기가 뒤집히는 것을 방지하기 위해 로컬 Y축을 뒤집어줍니다.
                // 2D에서 왼쪽을 보고 각도가 위아래로 움직일 때 무기가 거꾸로 들리는 걸 막아줍니다.
                transform.localRotation = Quaternion.Euler(0, 180, -targetAngle);
            }
            else
            {
                // 플레이어가 오른쪽을 볼 때는 일반적인 2D 회전을 적용하고 Y축 회전을 초기화합니다.
                transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }
    }
}