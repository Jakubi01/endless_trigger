using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class CoolTimePanel : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TMP_Text coolTimeText;

        public void SetWeaponImage(Sprite image)
        {
            if (!weaponImage || !image) return;

            weaponImage.sprite = image;
        }
        
        public void SetCoolTimeText(float coolTime)
        {
            if (!coolTimeText) return;
            
            coolTimeText.text = coolTime.ToString("F2", CultureInfo.CurrentCulture);
        }
    }
}
