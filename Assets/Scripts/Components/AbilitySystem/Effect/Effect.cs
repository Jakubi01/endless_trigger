using UnityEngine;

namespace Components.AbilitySystem.Effect
{
    public abstract class EffectBase : ScriptableObject
    {
        public abstract void Apply(AbilitySystemComponent target);
    }
    
    [CreateAssetMenu(menuName = "Effects/ExecuteSpeedEffect")]
    public class ExecuteSpeedEffect : EffectBase
    {
        [SerializeField] private AbilityType targetType;
        [SerializeField] private float bonus = 0.1f;

        public override void Apply(AbilitySystemComponent target)
        {
            foreach (var ability in target.Abilities)
                if (ability.AbilityType == targetType)
                    ability.AddExecuteSpeedBonus(bonus);
        }
    }

    [CreateAssetMenu(menuName = "Effects/UpgradeAbilityEffect")]
    public class UpgradeAbilityEffect : EffectBase
    {
        [SerializeField] private AbilityType targetType;

        public override void Apply(AbilitySystemComponent target)
        {
            foreach (var ability in target.Abilities)
                if (ability.AbilityType == targetType)
                    ability.Upgrade();
        }
    }
}