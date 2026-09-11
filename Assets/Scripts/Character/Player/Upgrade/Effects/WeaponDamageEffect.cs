using Items.Weapon;
using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Weapon Damage")]
    public class WeaponDamageEffect : PlayerUpgradeEffect
    {
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +n weapon damage
            // amount: n
            // maxStacks: 10
            // weight: 3
            
            if(!player) return;
            
            foreach (var weapon in player.EquippedWeapons)
                weapon.AddDamageBonus(amount);
        }
    }
}