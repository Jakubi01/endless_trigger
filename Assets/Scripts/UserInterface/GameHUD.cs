using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameHUD : UserInterface
    {
        [SerializeField] private Slider hpBarSlider;
        [SerializeField] private Slider expBarSlider;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image shotgunCooldownImage;
        [SerializeField] private Image sniperCooldownImage;
    }
}