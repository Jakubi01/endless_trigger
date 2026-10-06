using Items.ProjectileManager;
using Items.Weapon;
using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Shield Ability", menuName = "Endless Trigger/Abilities/Shield Ability")]
    public class ShieldAbility : AbilityBase
    {
        /**
         * Execute: 캐릭터 주변 5m 범위를 도는 방패 1개 생성, 속도 10 m/s(이건 수치 조정이 필요함)
         * Upgrade: 방패 추가 +1, 데미지 +20
         */

        private int _shieldCount;

        private float _damage = 20f;
        private ProjectilePoolManager _projectilePoolManager;
        
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner); 

            var shieldAttack = Owner.transform.Find("ShieldAttack");
            if (shieldAttack)
            {
                _projectilePoolManager = shieldAttack.GetComponent<ProjectilePoolManager>();
            }

            _shieldCount = 1;
            _damage = 20f;
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            float startAngle = _shieldCount == 4 ? 45f : 90f;
            float step = 360f / _shieldCount;

            for (int i = 0; i < _shieldCount; i++)
            {
                GameObject go = _projectilePoolManager.Spawn(Owner.transform.position, Quaternion.identity, Vector2.zero, _damage, pierceCount: 999);
                if (go.TryGetComponent(out Shield shield))
                {
                    shield.Init(Owner.transform, startAngle + step * i);
                }
            }

            return true;
        }
        
        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);

            _shieldCount++;
            _damage += 20f;
        }
    }
}