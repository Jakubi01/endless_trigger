using Components.AbilitySystem;
using Components.AbilitySystem.Abilities;
using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "GiveAbilityEffect", menuName = "Endless Trigger/Upgrade Effects/Give Ability")]
    public class GiveAbilityEffect : PlayerUpgradeEffect
    {
        [SerializeField] private AbilityBase ability;
        [SerializeField] private PlayerUpgradeDefinition ownedAbilityUpgrade;
 
        public override PlayerUpgradeDefinition ResolveDefinition(
            PlayerCharacter player,
            PlayerUpgradeDefinition definition)
        {
            var system = player ? player.GetComponent<AbilitySystem>() : null;
            return system && ability && system.HasAbility(ability.AbilityType) && ownedAbilityUpgrade
                ? ownedAbilityUpgrade
                : definition;
        }
        
        public override bool CanApply(PlayerCharacter player)
        {
            var system = player ? player.GetComponent<AbilitySystem>() : null;
            return system && ability && !system.HasAbility(ability.AbilityType);
        }
        
        public override void Apply(PlayerCharacter player, float amount)
        {
            var system = player ? player.GetComponent<AbilitySystem>() : null;
            if (!system) return;
            
            system.AddAbility(ability);
        }
    }
}
