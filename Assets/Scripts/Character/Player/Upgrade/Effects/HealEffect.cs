using Components;
using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Heal")]
    public class HealEffect : PlayerUpgradeEffect
    {
        private enum HealType
        {
            Health,
            MaxHealth
        }
        
        [SerializeField] private HealType healType;
        
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +n max health and heal the same amount
            // amount: n
            // maxStacks: 8
            // weight: 2

            if (!player) return;

            var healthComponent = player.GetComponent<HealthComponent>();
            if(!healthComponent) return;
            
            switch (healType)
            {
                case HealType.Health:
                    healthComponent.Heal(amount);
                    break;
                
                case HealType.MaxHealth:
                    healthComponent.AddMaxHealth(amount);
                    break;
            }

        }
    }
}