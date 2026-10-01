using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Settings
{
    public class GameplaySettingWindow : UserInterface, ISettingWindow
    {
        [Header("Gameplay UI")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle screenShakeToggle;
        [SerializeField] private Text   screenShakeToggleText;

        public WindowType Type { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            InitToggleValue();
            InitSlider();
            RegisterUIEvents();

            Type = WindowType.Gameplay;
        }

        private void OnEnable()
        {
            SettingManager.Instance.BackupCurrentSettings();
            UpdateUIFromManager();
        }

        private void OnDestroy()
        {
            UnregisterUIEvents();
        }
        
        private void InitSlider()
        {
            var sensitivity = sensitivitySlider.fillRect.GetComponent<Image>();
            sensitivity.type = Image.Type.Filled;
            sensitivity.fillMethod = Image.FillMethod.Horizontal;
            sensitivity.fillOrigin = 0;
        }

        private void InitToggleValue()
        {
            SetToggleValue(screenShakeToggle, true);
            SetToggleText(screenShakeToggle, screenShakeToggleText);
        }

        private void RegisterUIEvents()
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            screenShakeToggle.onValueChanged.AddListener(OnScreenShakeToggleChanged);
        }

        private void UnregisterUIEvents()
        {
            sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            screenShakeToggle.onValueChanged.RemoveListener(OnScreenShakeToggleChanged);
        }

        private void UpdateUIFromManager()
        {
            var manager = SettingManager.Instance;

            sensitivitySlider.SetValueWithoutNotify(manager.MouseSensitivity);
            screenShakeToggle.SetIsOnWithoutNotify(manager.UseScreenShake);
        }
        
        private void OnSensitivityChanged(float val) => SettingManager.Instance.SetMouseSensitivity(val);

        private void OnScreenShakeToggleChanged(bool val)
        { 
            SettingManager.Instance.SetScreenShake(val);
            SetToggleText(screenShakeToggle, screenShakeToggleText);
        }
    }
}