using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Attack Speed")]
    public class AttackSpeedEffect : PlayerUpgradeEffect
    {
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +15% attack speed for all weapons or weapon.
            // amount: 0.15
            // max stacks: 8
            // weight: 2
            
        }
    }
}