using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class TitleWindow : UserInterface
    {
        [SerializeField] private Button titleAnimationButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button playerProfileButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private PlayerProfilePopup playerProfilePopup;

        private bool _widgetVisible = true;

        protected override void Awake()
        {
            base.Awake();
            
            var cs = GetComponent<CanvasScaler>();
            if (!cs)
            {
                gameObject.AddComponent<CanvasScaler>();
                if (!cs) return;
            }

            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cs.matchWidthOrHeight = 0.5f;
            cs.referencePixelsPerUnit = 100f;
        }
        
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

            if (playerProfileButton)
            {
                playerProfilePopup ??= transform.GetComponentInChildren<PlayerProfilePopup>();
                playerProfileButton.onClick.AddListener(playerProfilePopup.Popup);
                playerProfileButton.onClick.AddListener(() =>
                {
                    _widgetVisible = !_widgetVisible;
                    
                    titleAnimationButton.gameObject.SetActive(_widgetVisible);
                    startButton.gameObject.SetActive(_widgetVisible);
                    settingButton.gameObject.SetActive(_widgetVisible);
                    exitButton.gameObject.SetActive(_widgetVisible);
                });
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

            if (playerProfileButton)
            {
                playerProfileButton.onClick.RemoveAllListeners();
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
