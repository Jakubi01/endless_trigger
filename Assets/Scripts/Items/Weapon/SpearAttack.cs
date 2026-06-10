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
            angle -= 90f;

            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector3 muzzlePosition = muzzlePoint ? muzzlePoint.position : transform.position;
            ProjectileManager.Spawn(muzzlePosition, rotation, direction, owner, Damage, pierceCount);
        }

        protected override void UpdateTarget()
        {
            if (!owner) return;
            
            GameObject farthest = owner.FindFarthestFromCharacter(range);
            CurrentTarget = farthest ? farthest.GetComponent<EnemyCharacterBase>() : null;
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
