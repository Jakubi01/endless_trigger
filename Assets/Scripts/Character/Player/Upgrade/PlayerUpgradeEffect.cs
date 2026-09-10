using UnityEngine;

namespace Character.Player.Upgrade
{
    public abstract class PlayerUpgradeEffect : ScriptableObject
    {
        public abstract void Apply(PlayerCharacter player, float amount);
    }
}