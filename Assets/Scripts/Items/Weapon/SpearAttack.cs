using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    public class SpearAttack : WeaponAttackBase
    {
        [Header("Spear Attack Settings")]
        [SerializeField] private float range = 20f;
        [SerializeField] private int pierceCount = 1;
        [SerializeField] private Transform muzzlePoint;

        protected override void Attack()
        {
            if (!owner) return;

            Vector2 direction = (CurrentTarget.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector3 muzzlePosition = muzzlePoint ? muzzlePoint.position : transform.position;
            ProjectileManager.Spawn(muzzlePosition, rotation, direction, Damage, pierceCount);
        }

        protected override void LookAtTarget()
        {
            if (!owner) return;
            
            GameObject farthest = owner.FindFarthestFromCharacter(range);
            CurrentTarget = farthest ? farthest.GetComponent<EnemyCharacterBase>() : null;
            if (!CurrentTarget) return;

            Vector3 targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
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
            pierceCount++;
            AddDamage(40f);
            AddCooldown(-0.3f);
        }

        protected override float GetDefaultDamage()
        {
            return 40f;
        }
    }
}
