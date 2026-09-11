using Items.Weapon;
using UnityEngine;

namespace Character.Player.Upgrade.Effects
{
    [CreateAssetMenu(fileName = "HealUpgradeEffect", menuName = "Endless Trigger/Upgrade Effects/Attack Speed")]
    public class AttackSpeedEffect : PlayerUpgradeEffect
    {
        private enum PlayerWeaponTarget
        {
            All,
            Sword,
            Spear
        }

        [SerializeField] private PlayerWeaponTarget target;
        
        public override void Apply(PlayerCharacter player, float amount)
        {
            // Desc: +15% attack speed for all weapons or weapon.
            // amount: 0.15
            // max stacks: 8
            // weight: 2

            if (!player) return;

            if (target == PlayerWeaponTarget.All)
            {
                foreach (var weapon in player.EquippedWeapons)
                    weapon.AddAttackSpeedBonus(amount);
                
                return;
            }
            
            
            WeaponAttackBase targetWeapon = null;
            foreach (var weapon in player.EquippedWeapons)
            {
                if ((target == PlayerWeaponTarget.Sword && weapon is SwordAttack) ||
                    (target == PlayerWeaponTarget.Spear && weapon is SpearAttack))
                {
                    targetWeapon = weapon;
                    break;
                }
            }

            if (targetWeapon) 
                targetWeapon.AddAttackSpeedBonus(amount);
        }
    }
}