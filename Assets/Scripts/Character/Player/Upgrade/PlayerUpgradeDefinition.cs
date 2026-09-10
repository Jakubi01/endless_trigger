using System.Collections.Generic;
using Character.Player.Upgrade;
using UnityEngine;

namespace Character.Player
{
    /// <summary>
    /// 레벨업 카드 하나의 데이터입니다.
    /// 실제 효과 구현은 PlayerUpgradeEffect에게 위임합니다.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PlayerUpgrade",
        menuName = "Endless Trigger/Player Upgrade")]
    public class PlayerUpgradeDefinition : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string displayName = "New Upgrade";

        [TextArea]
        [SerializeField] private string description;

        [SerializeField] private Sprite icon;

        [Header("Upgrade")]
        [SerializeField] private PlayerUpgradeEffect effect;

        [Min(0.01f)]
        [SerializeField] private float amount = 1f;

        [Min(1)]
        [SerializeField] private int maxStacks = 5;

        [Min(1)]
        [SerializeField] private int weight = 1;

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int Weight => weight;

        /// <summary>
        /// 현재 플레이어에게 이 업그레이드를 적용할 수 있는지 확인합니다.
        /// </summary>
        public bool CanApply(PlayerCharacter player)
        {
            if (!player)
                return false;

            if (!effect)
                return false;

            return player.GetUpgradeStackCount(this) < maxStacks;
        }

        /// <summary>
        /// 업그레이드를 플레이어에게 적용합니다.
        /// 실제 구현은 PlayerUpgradeEffect가 담당합니다.
        /// </summary>
        public void Apply(PlayerCharacter player)
        {
            if (!player)
                return;

            if (!effect)
            {
                Debug.LogWarning($"PlayerUpgradeDefinition '{name}'에 Upgrade Effect가 지정되지 않았습니다.", this);
                return;
            }

            effect.Apply(player, amount);
        }

        /// <summary>
        /// 등록된 업그레이드 중 랜덤한 선택지를 반환합니다.
        /// Weight를 이용한 가중치 랜덤을 사용합니다.
        /// </summary>
        public static List<PlayerUpgradeDefinition> GetRandomChoices(
            PlayerCharacter player,
            IEnumerable<PlayerUpgradeDefinition> definitions,
            int requestedCount)
        {
            List<PlayerUpgradeDefinition> candidates = new();

            if (!player || definitions == null || requestedCount <= 0)
                return candidates;

            foreach (PlayerUpgradeDefinition definition in definitions)
            {
                if (!definition)
                    continue;

                if (!definition.CanApply(player))
                    continue;

                if (candidates.Contains(definition))
                    continue;

                candidates.Add(definition);
            }

            List<PlayerUpgradeDefinition> choices = new();

            while (candidates.Count > 0 && choices.Count < requestedCount)
            {
                int totalWeight = 0;

                foreach (PlayerUpgradeDefinition definition in candidates)
                {
                    totalWeight += Mathf.Max(1, definition.Weight);
                }

                if (totalWeight <= 0)
                    break;

                int roll = Random.Range(0, totalWeight);
                int selectedIndex = 0;

                for (; selectedIndex < candidates.Count - 1; selectedIndex++)
                {
                    roll -= Mathf.Max(1, candidates[selectedIndex].Weight);

                    if (roll < 0)
                        break;
                }

                choices.Add(candidates[selectedIndex]);
                candidates.RemoveAt(selectedIndex);
            }

            return choices;
        }
    }
}