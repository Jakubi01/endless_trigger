using System;
using Character.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class SelectCard : MonoBehaviour
    {
        [SerializeField] private Button clickButton;       // 클릭을 감지할 버튼 컴포넌트
        [SerializeField] private Image iconImage;          // 아이콘 이미지
        [SerializeField] private TMP_Text nameText;        // 스킬명 텍스트
        [SerializeField] private TMP_Text descriptionText; // 설명 텍스트

        ///<summary>
        /// 전달받은 데이터로 UI 요소들을 채우고 버튼 이벤트를 등록합니다.
        /// </summary>
        public void Setup(PlayerUpgradeDefinition definition, Action onClickAction)
        {
            if (nameText) nameText.text = definition.DisplayName;
            if (descriptionText) descriptionText.text = definition.Description;
            
            if (iconImage)
            {
                if (definition.Icon)
                {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = definition.Icon;
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }

            if (!clickButton) return;
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(() => onClickAction?.Invoke());
        }
    }
}