using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class SettingWindow : UserInterface
    {
        [Header("Audio UI")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle bgmToggle;
        [SerializeField] private Text   bgmToggleText;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Text   muteToggleText;

        [Header("Graphics UI")]
        [SerializeField] private Toggle windowModeToggle;
        [SerializeField] private Text   windowModeToggleText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown frameRateDropdown;

        [Header("Gameplay UI")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle screenShakeToggle;
        [SerializeField] private Text   screenShakeToggleText;

        [Header("Panels")]
        [SerializeField] private GameObject creditsPanel;

        private readonly List<Resolution> _systemResolutions = new();

        private void Awake()
        {
            InitResolutionDropdown();
            InitQualityDropdown();
            InitFrameRateDropdown();
            InitSlider();
            InitToggleValue();
            RegisterUIEvents();
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

        private void InitResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            _systemResolutions.Clear();
            _systemResolutions.AddRange(Screen.resolutions);

            List<string> options = new List<string>();
            int maxResIndex = 0;
            long maxPixelCount = 0;
            double maxRefreshRate = 0;

             for (int i = 0; i < _systemResolutions.Count; i++)
            {
                var res = _systemResolutions[i];
                string option = $"{res.width} x {res.height} @ {res.refreshRateRatio.value:F0}Hz";
                options.Add(option);

                // 최대 해상도 및 주사율 탐색 (너비x높이가 크거나, 같으면 주사율이 더 높은 항목 선택)
                long currentPixelCount = (long)res.width * res.height;
                double currentRefreshRate = res.refreshRateRatio.value;

                if (currentPixelCount > maxPixelCount || 
                    (currentPixelCount == maxPixelCount && currentRefreshRate > maxRefreshRate))
                {
                    maxPixelCount = currentPixelCount;
                    maxRefreshRate = currentRefreshRate;
                    maxResIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);

            int savedResIndex = SettingManager.Instance.ResolutionIndex;
            if (savedResIndex >= 0 && savedResIndex < _systemResolutions.Count)
            {
                resolutionDropdown.SetValueWithoutNotify(savedResIndex);
            }
            else
            {
                // 저장된 설정이 없을 때 최대 해상도로 지정
                resolutionDropdown.SetValueWithoutNotify(maxResIndex);
                SettingManager.Instance.SetResolution(maxResIndex);
            }

            resolutionDropdown.RefreshShownValue();
        }

        private void InitQualityDropdown()
        {
            qualityDropdown.ClearOptions();

            List<string> options = new(QualitySettings.names);
            qualityDropdown.AddOptions(options);

            qualityDropdown.SetValueWithoutNotify(GetValidQualityIndex(SettingManager.Instance.QualityIndex));
            qualityDropdown.RefreshShownValue();
        }

        private void InitFrameRateDropdown()
        {
            frameRateDropdown.ClearOptions();

            List<string> options = new List<string>();
            foreach (FrameRateMode mode in Enum.GetValues(typeof(FrameRateMode)))
            {
                options.Add(GetFrameRateLabel(mode));
            }

            frameRateDropdown.AddOptions(options);
            frameRateDropdown.SetValueWithoutNotify(GetValidFrameRateIndex((int)SettingManager.Instance.CurrentFrameRateMode));
            frameRateDropdown.RefreshShownValue();
        }

        private void InitSlider()
        {
            var master = masterVolumeSlider.fillRect.GetComponent<Image>();
            master.type = Image.Type.Filled;
            master.fillMethod =  Image.FillMethod.Horizontal;
            master.fillOrigin = 0;
            
            var bgm = bgmVolumeSlider.fillRect.GetComponent<Image>();
            bgm.type = Image.Type.Filled;
            bgm.fillMethod =  Image.FillMethod.Horizontal;
            bgm.fillOrigin = 0;

            var sfx = sfxVolumeSlider.fillRect.GetComponent<Image>();
            sfx.type = Image.Type.Filled;
            sfx.fillMethod =   Image.FillMethod.Horizontal;
            sfx.fillOrigin = 0;
            
            var sensitivity = sensitivitySlider.fillRect.GetComponent<Image>();
            sensitivity.type = Image.Type.Filled;
            sensitivity.fillMethod = Image.FillMethod.Horizontal;
            sensitivity.fillOrigin = 0;
        }

        private void InitToggleValue()
        {
            SetToggleValue(bgmToggle, true);
            SetToggleText(bgmToggle, bgmToggleText);

            SetToggleValue(muteToggle, false);
            SetToggleText(muteToggle, muteToggleText);

            bool isWindowMode = SettingManager.Instance.CurrentScreenMode == ScreenMode.Windowed;
            SetToggleValue(windowModeToggle, isWindowMode);
            SetToggleText(windowModeToggle, windowModeToggleText);

            SetToggleValue(screenShakeToggle, true);
            SetToggleText(screenShakeToggle, screenShakeToggleText);
        }

        private void RegisterUIEvents()
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);
            bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);

            windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);

            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            screenShakeToggle.onValueChanged.AddListener(OnScreenShakeToggleChanged);
        }

        private void UnregisterUIEvents()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            bgmToggle.onValueChanged.RemoveListener(OnBgmToggleChanged);
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            muteToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);

            windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);
            frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateChanged);

            sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            screenShakeToggle.onValueChanged.RemoveListener(OnScreenShakeToggleChanged);
        }

        private void UpdateUIFromManager()
        {
            var manager = SettingManager.Instance;

            masterVolumeSlider.SetValueWithoutNotify(manager.MasterVolume);
            bgmToggle.SetIsOnWithoutNotify(manager.IsBgmOn);
            bgmVolumeSlider.SetValueWithoutNotify(manager.BgmVolume);
            bgmVolumeSlider.interactable = manager.IsBgmOn;
            sfxVolumeSlider.SetValueWithoutNotify(manager.SfxVolume);
            muteToggle.SetIsOnWithoutNotify(manager.IsMute);

            windowModeToggle.SetIsOnWithoutNotify(manager.CurrentScreenMode == ScreenMode.Windowed);
            resolutionDropdown.SetValueWithoutNotify(GetValidResolutionIndex(manager.ResolutionIndex));
            qualityDropdown.SetValueWithoutNotify(GetValidQualityIndex(manager.QualityIndex));
            frameRateDropdown.SetValueWithoutNotify(GetValidFrameRateIndex((int)manager.CurrentFrameRateMode));
            resolutionDropdown.RefreshShownValue();
            qualityDropdown.RefreshShownValue();
            frameRateDropdown.RefreshShownValue();

            sensitivitySlider.SetValueWithoutNotify(manager.MouseSensitivity);
            screenShakeToggle.SetIsOnWithoutNotify(manager.UseScreenShake);
        }

        private void SetToggleValue(Toggle toggle, bool value)
        {
            toggle.isOn = value;
        }
        
        private void SetToggleText(in Toggle toggle, Text text)
        {
            text.text = toggle.isOn ? "ON" : "OFF";
        }

        private void OnMasterVolumeChanged(float val) => SettingManager.Instance.SetMasterVolume(val);

        private void OnBgmToggleChanged(bool val)
        {
            SettingManager.Instance.SetBgmOn(val);
            SetToggleText(bgmToggle, bgmToggleText);
            bgmVolumeSlider.interactable = val;
        }

        private void OnBgmVolumeChanged(float val) => SettingManager.Instance.SetBgmVolume(val);
        private void OnSfxVolumeChanged(float val) => SettingManager.Instance.SetSfxVolume(val);

        private void OnMuteToggleChanged(bool val)
        {
            SettingManager.Instance.SetMute(val);
            SetToggleText(muteToggle, muteToggleText);
        }

        private void OnWindowModeToggleChanged(bool val)
        {
            var manager = SettingManager.Instance;
    
            manager.SetScreenMode(val ? ScreenMode.Windowed : ScreenMode.FullScreen);
            SetToggleText(windowModeToggle, windowModeToggleText);

            if (val)
            { 
                resolutionDropdown.interactable = true; 
                return;
            }
            
            var maxIndex = manager.GetMaxResolutionIndex();
            manager.SetResolution(maxIndex);

            resolutionDropdown.SetValueWithoutNotify(maxIndex);
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.interactable = false; 
        }

        private void OnResolutionChanged(int index) => SettingManager.Instance.SetResolution(index);
        private void OnQualityChanged(int index) => SettingManager.Instance.SetQuality(index);
        private void OnFrameRateChanged(int index) => SettingManager.Instance.SetFrameRate((FrameRateMode)index);

        private void OnSensitivityChanged(float val) => SettingManager.Instance.SetMouseSensitivity(val);

        private void OnScreenShakeToggleChanged(bool val)
        { 
            SettingManager.Instance.SetScreenShake(val);
            SetToggleText(screenShakeToggle, screenShakeToggleText);
        }

        public void OnClickApply()
        {
            SettingManager.Instance.SaveSettings();
            CloseWindow();
        }

        public void OnClickCancel()
        {
            SettingManager.Instance.RevertSettings();
            CloseWindow();
        }

        public void OnClickOpenCredits() => creditsPanel.SetActive(true);
        public void OnClickCloseCredits() => creditsPanel.SetActive(false);

        private void CloseWindow()
        {
            if (UIManager.Instance)
            {
                UIManager.Instance.OnSettingWindowClosed(this);
                return;
            }

            gameObject.SetActive(false);
        }

        private int GetValidResolutionIndex(int index)
        {
            if (_systemResolutions.Count == 0)
                return 0;

            return Mathf.Clamp(index, 0, _systemResolutions.Count - 1);
        }

        private int GetValidQualityIndex(int index)
        {
            int maxIndex = Mathf.Max(0, QualitySettings.names.Length - 1);
            return Mathf.Clamp(index, 0, maxIndex);
        }

        private int GetValidFrameRateIndex(int index)
        {
            int maxIndex = Enum.GetValues(typeof(FrameRateMode)).Length - 1;
            return Mathf.Clamp(index, 0, maxIndex);
        }

        private string GetFrameRateLabel(FrameRateMode mode)
        {
            return mode switch
            {
                FrameRateMode.FPS30 => "30 FPS",
                FrameRateMode.FPS60 => "60 FPS",
                FrameRateMode.FPS120 => "120 FPS",
                FrameRateMode.FPS240 => "240 FPS",
                FrameRateMode.FPS300 => "300 FPS",
                FrameRateMode.Uncapped => "Uncapped",
                _ => mode.ToString()
            };
        }
    }
}
