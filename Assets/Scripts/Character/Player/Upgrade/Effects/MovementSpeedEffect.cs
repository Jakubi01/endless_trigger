using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Movement Speed")]
    public class MovementSpeedEffect : PlayerUpgradeEffect
    {
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +0.5 movement speed
            // amount: 0.5
            // masStacks: 8
            // weight: 2
            if(!player) return;

            player.AddMoveSpeed(amount);
        }
    }
}