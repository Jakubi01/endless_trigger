using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class TitleWindow : UserInterface
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            if (startButton)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }

            if (settingButton)
            {
                settingButton.onClick.AddListener(OnSettingButtonClicked);
            }

            if (exitButton)
            {
                exitButton.onClick.AddListener(OnExitButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (startButton)
            {
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            }

            if (settingButton)
            {
                settingButton.onClick.RemoveListener(OnSettingButtonClicked);
            }

            if (exitButton)
            {
                exitButton.onClick.RemoveListener(OnExitButtonClicked);
            }
        }

        private void OnStartButtonClicked()
        {
            UIManager.Instance.OnStartButtonClicked();
        }

        private void OnSettingButtonClicked()
        {
            UIManager.Instance.OnSettingsButtonClicked();
        }

        private void OnExitButtonClicked()
        {
            UIManager.Instance.OnExitButtonClicked();
        }
    }
}
