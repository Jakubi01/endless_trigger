using Components.AbilitySystem;
using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "UpgradeAbilityEffect", menuName = "Endless Trigger/Upgrade Effects/Upgrade Ability")]
    public class UpgradeAbilityEffect : PlayerUpgradeEffect
    {
        [SerializeField] private AbilityType targetType;

        public override bool CanApply(PlayerCharacter player)
        {
            var system = player ? player.GetComponent<AbilitySystem>() : null;
            return system && system.HasAbility(targetType);
        }

        public override void Apply(PlayerCharacter player, float amount)
        {
            var system = player ? player.GetComponent<AbilitySystem>() : null;
            if (system && system.TryGetAbility(targetType, out var ability))
                ability.Upgrade(amount);
        }
    }
}
