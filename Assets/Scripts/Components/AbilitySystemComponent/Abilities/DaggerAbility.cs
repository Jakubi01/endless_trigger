using System.Collections.Generic;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Components.AbilitySystemComponent.Abilities
{
    public class DaggerAbility : AbilityBase
    {
        [SerializeField] private Dagger daggerPrefab;

        private int _daggerCount;
        private float _spreadAngle;
        private float _damage;
        private float _range;
        private readonly HashSet<GameObject> _targets = new();
        
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);

            _daggerCount = 5;
            _damage = 10f;
            _spreadAngle = 60f;
            _range = 6f;
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            _targets.Clear();

            var target = EnemyManager.Instance.FindNearestEnemy(Owner.transform, _range, _targets);
            
            Vector2 aimDir;
            if (target is not null)
            {
                aimDir = (target.transform.position - Owner.transform.position).normalized;
                _targets.Add(target);
            }
            else
            {
                aimDir = Owner.transform.right;
            }

            var startAngle = -_spreadAngle / 2f;
            var angleStep = _daggerCount > 1
                ? _spreadAngle / (_daggerCount - 1)
                : 0f;

            for (var i = 0; i < _daggerCount; i++)
            {
                var angle = startAngle + angleStep * i;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * aimDir;

                var dagger = Instantiate(daggerPrefab, Owner.transform.position, Quaternion.identity);
                if (!dagger) continue;

                dagger.Init(direction);
                dagger.SetDamage(_damage);
            }

            return true;
        }
        
        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);
            
            _daggerCount += 2;
            _damage += 15f;
        }
    }
}