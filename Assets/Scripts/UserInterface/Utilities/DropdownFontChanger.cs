using TMPro;
using UnityEngine;

namespace UserInterface.Utilities
{
    public class DropdownFontChanger : MonoBehaviour
    {
        [Header("Dropdown Font")]
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI itemText;
        [SerializeField] private TMP_FontAsset targetFont;

        private void Awake()
        {
            if (!targetFont || !itemText || !captionText) return;

            captionText.font = targetFont;
            itemText.font = targetFont;
        }
    }
}