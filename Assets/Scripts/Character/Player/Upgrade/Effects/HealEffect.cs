using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Heal")]
    public class HealEffect : PlayerUpgradeEffect
    {
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +n max health and heal the same amount
            // amount: n
            // maxStacks: 8
            // weight: 2
            
        }
    }
}