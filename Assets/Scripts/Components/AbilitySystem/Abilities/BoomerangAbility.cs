using System.Collections.Generic;
using Character;
using Items.ProjectileManager;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Boomerang Ability", menuName = "Endless Trigger/Abilities/Boomerang Ability")]
    public class BoomerangAbility : AbilityBase
    {
        /**
         * Execute: 부메랑 투척 후 캐릭터에게 돌아옴, 사거리 10m, 돌아오면 다시 투척 -> 쿨타임 x?
         * Upgrade: 발사 횟수 +1, 데미지 +20, 속도 증가 +10
         */
        
        private float _damage = 20f;

        private ProjectilePoolManager _projectilePoolManager;
        private int _throwCount;
        private readonly HashSet<GameObject> _targets = new();

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner); 

            var boomerangAttack = Owner.transform.Find("BoomerangAttack");
            if (boomerangAttack)
            {
                _projectilePoolManager = boomerangAttack.GetComponent<ProjectilePoolManager>();
            }

            _damage = 20f;
            _throwCount = 1;
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            _targets.Clear();
            for (int i = 0; i < _throwCount; i++)
            {
                var target = EnemyManager.Instance.FindNearestEnemy(Owner.transform, 5f, _targets);

                Vector2 aimDir;
                if (target)
                {
                    _targets.Add(target);
                    aimDir = (target.transform.position - Owner.transform.position).normalized;
                }
                else
                {
                    aimDir = Random.insideUnitCircle.normalized;
                }

                GameObject go = _projectilePoolManager.Spawn(Owner.transform.position, Quaternion.identity, aimDir, _damage, pierceCount: 999);
                if (go.TryGetComponent(out Boomerang boomerang))
                {
                    boomerang.Throw(Owner.transform, aimDir);
                }
            }
            return true;
        }

        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);

            _damage += 20f;
            _throwCount++;
        }
    }
}