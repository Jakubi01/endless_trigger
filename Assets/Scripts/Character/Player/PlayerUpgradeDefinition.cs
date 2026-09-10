using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public enum PlayerUpgradeEffect
    {
        MaxHealth,
        Heal,
        MoveSpeed,
        WeaponDamage,
        WeaponAttackSpeed
    }

    public enum PlayerWeaponTarget
    {
        All,
        Sword,
        Spear
    }

    /// <summary>
    /// 레벨업 카드 하나의 데이터입니다. 새 효과는 enum과 Apply switch에만 추가하면 됩니다.
    /// Create 메뉴로 에셋을 만든 뒤 PlayerCharacter의 upgradeDefinitions 목록에 등록합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "Endless Trigger/Player Upgrade")]
    public class PlayerUpgradeDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "New Upgrade";
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private PlayerUpgradeEffect effect;
        [SerializeField] private PlayerWeaponTarget weaponTarget = PlayerWeaponTarget.All;
        [Min(0.01f)] [SerializeField] private float amount = 1f;
        [Min(1)] [SerializeField] private int maxStacks = 5;
        [Min(1)] [SerializeField] private int weight = 1;

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int Weight => weight;

        public bool CanApply(PlayerCharacter player)
        {
            return player && player.GetUpgradeStackCount(this) < maxStacks;
        }

        public void Apply(PlayerCharacter player)
        {
            if (!player) return;

            switch (effect)
            {
                case PlayerUpgradeEffect.MaxHealth:
                    player.AddMaxHealth(amount);
                    break;
                case PlayerUpgradeEffect.Heal:
                    player.Heal(amount);
                    break;
                case PlayerUpgradeEffect.MoveSpeed:
                    player.AddMoveSpeed(amount);
                    break;
                case PlayerUpgradeEffect.WeaponDamage:
                    player.UpgradeWeaponDamage(weaponTarget, amount);
                    break;
                case PlayerUpgradeEffect.WeaponAttackSpeed:
                    player.UpgradeWeaponAttackSpeed(weaponTarget, amount);
                    break;
            }
        }

        public static List<PlayerUpgradeDefinition> GetRandomChoices(
            PlayerCharacter player,
            IEnumerable<PlayerUpgradeDefinition> definitions,
            int requestedCount)
        {
            List<PlayerUpgradeDefinition> candidates = new();
            foreach (PlayerUpgradeDefinition definition in definitions)
            {
                if (definition && definition.CanApply(player) && !candidates.Contains(definition))
                {
                    candidates.Add(definition);
                }
            }
            List<PlayerUpgradeDefinition> choices = new();

            while (candidates.Count > 0 && choices.Count < requestedCount)
            {
                int totalWeight = 0;
                foreach (PlayerUpgradeDefinition definition in candidates)
                {
                    totalWeight += definition.Weight;
                }
                int roll = Random.Range(0, totalWeight);
                int selectedIndex = 0;
                for (; selectedIndex < candidates.Count - 1; selectedIndex++)
                {
                    roll -= candidates[selectedIndex].Weight;
                    if (roll < 0) break;
                }

                choices.Add(candidates[selectedIndex]);
                candidates.RemoveAt(selectedIndex);
            }

            return choices;
        }
    }
}
