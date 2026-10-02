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
        
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            return true;
        }
        
        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);
            
            
        }
    }
}