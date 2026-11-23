using Items.ProjectileManager;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Components.AbilitySystemComponent.Abilities
{
    [CreateAssetMenu(fileName = "Dagger Ability", menuName = "Endless Trigger/Abilities/Dagger Ability")]
    public class DaggerAbility : AbilityBase
    {
        private const string DaggerAttackObjectName = "DaggerAttack";

        [SerializeField, Min(1)] private int baseDaggerCount = 5;
        [SerializeField, Min(0f)] private float baseDamage = 10f;
        [SerializeField, Min(0f)] private float spreadAngle = 60f;
        [SerializeField, Min(0f)] private float range = 6f;

        private int _daggerCount;
        private float _damage;
        private ProjectilePoolManager _projectilePoolManager;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);

            var daggerAttack = Owner ? Owner.transform.Find(DaggerAttackObjectName) : null;
            _projectilePoolManager = daggerAttack
                ? daggerAttack.GetComponent<ProjectilePoolManager>()
                : null;

            _daggerCount = Mathf.Max(1, baseDaggerCount);
            _damage = baseDamage;
        }

        public override bool Execute()
        {
            if (!_projectilePoolManager || !base.Execute()) return false;

            var target = EnemyManager.Instance
                ? EnemyManager.Instance.FindNearestEnemy(Owner.transform, range)
                : null;
            if (target is null) return false;

            var aimDirection = (target.transform.position - Owner.transform.position).normalized;

            if (aimDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                aimDirection = Vector2.right;
            }

            SoundManagerInstance?.PlaySfx(executeClip);

            var startAngle = -spreadAngle / 2f;
            var angleStep = _daggerCount > 1 ? spreadAngle / (_daggerCount - 1) : 0f;

            for (var i = 0; i < _daggerCount; i++)
            {
                var angle = startAngle + angleStep * i;
                var direction = Quaternion.Euler(0f, 0f, angle) * aimDirection;

                var projectile = _projectilePoolManager.Spawn(
                    Owner.transform.position,
                    Quaternion.identity,
                    direction,
                    _damage);

                if (projectile && projectile.TryGetComponent(out Dagger dagger))
                {
                    dagger.Init(direction);
                }
            }

            return true;
        }

        public override void Upgrade(float amount)
        {
            if (amount <= 0f) return;

            base.Upgrade(amount);
            _daggerCount += 2;
            _damage += 15f;
        }
    }
}
