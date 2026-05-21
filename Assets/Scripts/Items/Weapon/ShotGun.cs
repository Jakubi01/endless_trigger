using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    public class ShotGun : WeaponBase
    {
        [Header("ShotGun Settings")]
        [SerializeField] private int bulletCount = 5;
        [SerializeField] private float spreadAngle = 30;
        [SerializeField] private float targetRange = 12f;
        [SerializeField] private Transform muzzlePoint;

        protected override void Fire()
        {
            Vector3 fireDirection = transform.right;

            float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            float startAngle = baseAngle - spreadAngle / 2f;
            float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;
            Vector3 muzzlePosition = muzzlePoint ? muzzlePoint.position : transform.position;

            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Vector2 direction = new Vector2(
                    Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                    Mathf.Sin(currentAngle * Mathf.Deg2Rad)
                );

                ProjectileManager.Spawn(muzzlePosition, rotation, direction, Damage, 0);
            }
        }

        protected override void LookAtTarget()
        {
            GameObject nearest = owner.FindNearestFromCharacter(targetRange);
            CurrentTarget = nearest ? nearest.GetComponent<EnemyCharacterBase>() : null;

            Vector3 targetDirection = owner.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            if (CurrentTarget)
            {
                targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
            }

            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

            if (owner.transform.localScale.x < 0)
            {
                targetAngle += 180f;
                transform.localRotation = Quaternion.Euler(0, 180, -targetAngle);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }

        public override void ApplyRandomUpgrade()
        {
            bulletCount++;
            AddDamage(5f);
            AddCooldown(-0.2f);
        }

        protected override float GetDefaultDamage()
        {
            return 10f;
        }
    }
}
