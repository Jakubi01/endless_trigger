using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Bomb Ability", menuName = "Endless Trigger/Abilities/Bomb Ability")]
    public class BombAbility : AbilityBase
    {
        /**
         * Execute: 10m 내 몬스터 x에게 1개 투척, 터지면 장판, 장판 데미지 초당 20 -> 5초 지속 --> 장판이 끝나면 다시 투척
         * Upgrade: 발사 횟수 +1, 장판데미지 +10, 범위 20% 증가
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