using UnityEngine;

namespace Character.Player.Upgrade
{
    public abstract class PlayerUpgradeEffect : ScriptableObject
    {
        public abstract void Apply(PlayerCharacter player, float amount);
        public virtual bool CanApply(PlayerCharacter player) => true;

        public virtual PlayerUpgradeDefinition ResolveDefinition(
            PlayerCharacter player,
            PlayerUpgradeDefinition definition) => definition;
    }
}
