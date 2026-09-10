using System;
using System.Collections.Generic;
using Character.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>프리팹 연결 없이 생성되는 기본 레벨업 카드 창입니다.</summary>
    public class LevelUpSelectionWindow : MonoBehaviour
    {
        private Action<PlayerUpgradeDefinition> _onSelected;
        private bool _selected;

        [SerializeField] private SelectCard[] cards;

        public void Setup(IReadOnlyList<PlayerUpgradeDefinition> choices, Action<PlayerUpgradeDefinition> onSelected)
        {
            _onSelected = onSelected;
            _selected = false;

            // 예외 처리: 데이터나 카드의 개수가 맞지 않을 경우를 대비
            if (choices == null || cards == null || choices.Count != cards.Length)
            {
                Debug.LogError($"선택지 데이터 개수({choices?.Count ?? 0})와 할당된 카드 UI 개수({cards?.Length ?? 0})가 일치하지 않습니다.");
                return;
            }

            for (int i = 0; i < cards.Length; i++)
            {
                PlayerUpgradeDefinition choice = choices[i];
                
                cards[i].Setup(choice, () => Select(choice));
            }
        }

        private void Select(PlayerUpgradeDefinition definition)
        {
            if (_selected) return;
            _selected = true;
            _onSelected?.Invoke(definition);
        }
    }
}
