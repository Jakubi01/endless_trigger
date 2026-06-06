using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    public class SwordAttack : WeaponBase
    {
        [Header("Sword Attack Settings")]
        [SerializeField] private float targetRange = 12f;
        [SerializeField] private int pierceCount = 1;
        [SerializeField] private Transform muzzlePoint;

        protected override void Attack()
        {
            Vector3 muzzlePosition = muzzlePoint ? muzzlePoint.position : transform.position;
            
            Vector3 direction = owner.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            if (CurrentTarget)
            {
                direction = (CurrentTarget.transform.position - transform.position).normalized;
            }
            
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, targetAngle);
            
            ProjectileManager.Spawn(muzzlePosition, rotation, direction, Damage, pierceCount);
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
            // pierceCount;;
            AddDamage(5f);
            AddCooldown(-0.2f);
        }

        protected override float GetDefaultDamage()
        {
            return 10f;
        }
    }
}
