using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    public class SwordAttack : WeaponAttackBase
    {
        [Header("Sword Attack Settings")]
        [SerializeField] private float targetRange = 4f;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private float attackOffset = 1.5f; // 플레이어 중심에서 슬래시가 생성될 반지름 거리
        private const int PierceCount = 999;                // 슬래시는 범위 내 적을 모두 베어야 하므로 관통 수를 높게 설정

        protected override void Attack()
        {
            if (!owner) return;
            
            Vector3 centerPosition = owner.transform.position;
            Vector3 direction = CurrentTarget ?
                (CurrentTarget.transform.position - centerPosition).normalized :
                owner.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            
            Vector3 spawnPosition = centerPosition + (direction * attackOffset);
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, targetAngle);
            
            if (muzzlePoint)
            {
                muzzlePoint.localPosition = spawnPosition - centerPosition;
            }
            
            Transform trackingTarget = muzzlePoint ? muzzlePoint : owner.transform;
            
            ProjectileManager.Spawn(spawnPosition, rotation, Vector2.zero, owner, Damage, PierceCount, true, trackingTarget);
        }

        protected override void UpdateTarget()
        {
            if (!owner) return;
            
            GameObject nearest = owner.FindNearestFromCharacter(targetRange);
            CurrentTarget = nearest ? nearest.GetComponent<EnemyCharacterBase>() : null;
        }

        public override void ApplyRandomUpgrade()
        {
            AddDamage(5f);
            AddCooldown(-0.2f);
        }

        protected override float GetDefaultDamage()
        {
            return 10f;
        }

        private void OnDrawGizmos()
        {
            if (!CurrentTarget) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(CurrentTarget.transform.position, new Vector3(2, 2, 2));
            Debug.Log(CurrentTarget.name);
        }
    }
}