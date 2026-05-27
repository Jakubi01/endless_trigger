using System.Collections.Generic;
using Managers;
using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class SettingWindow : MonoBehaviour
    {
        [Header("Audio UI")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle bgmToggle;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;

        [Header("Graphics UI")]
        [SerializeField] private Toggle windowModeToggle; 
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown frameRateDropdown;

        [Header("Gameplay UI")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle screenShakeToggle;

        [Header("Panels")]
        [SerializeField] private GameObject creditsPanel;

        private readonly List<Resolution> _systemResolutions = new();

        private void Awake()
        {
            InitResolutionDropdown();
        }

        private void OnEnable()
        {
            SettingManager.Instance.BackupCurrentSettings();
            UpdateUIFromManager();
        }

        private void InitResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            _systemResolutions.AddRange(Screen.resolutions);

            List<string> options = new List<string>();
            int currentResIndex = 0;

            for (int i = 0; i < _systemResolutions.Count; i++)
            {
                string option = $"{_systemResolutions[i].width} x {_systemResolutions[i].height} @ {_systemResolutions[i].refreshRateRatio.value:F0}Hz";
                options.Add(option);

                if (_systemResolutions[i].width == Screen.currentResolution.width &&
                    _systemResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);

            // 프로퍼티를 통해 우회 접근
            int savedResIndex = SettingManager.Instance.ResolutionIndex;
            if (savedResIndex >= 0 && savedResIndex < _systemResolutions.Count)
            {
                resolutionDropdown.value = savedResIndex;
            }
            else
            {
                resolutionDropdown.value = currentResIndex;
                SettingManager.Instance.SetResolution(currentResIndex);
            }

            resolutionDropdown.RefreshShownValue();
        }

        private void UpdateUIFromManager()
        {
            var manager = SettingManager.Instance;

            // 매니저의 Getter 프로퍼티에서 값을 안전하게 바인딩
            masterVolumeSlider.value = manager.MasterVolume;
            bgmToggle.isOn = manager.IsBgmOn;
            bgmVolumeSlider.value = manager.BgmVolume;
            bgmVolumeSlider.interactable = manager.IsBgmOn;
            sfxVolumeSlider.value = manager.SfxVolume;
            muteToggle.isOn = manager.IsMute;

            windowModeToggle.isOn = (manager.CurrentScreenMode == ScreenMode.Windowed);
            resolutionDropdown.value = manager.ResolutionIndex;
            qualityDropdown.value = manager.QualityIndex;
            frameRateDropdown.value = (int)manager.CurrentFrameRateMode;

            sensitivitySlider.value = manager.MouseSensitivity;
            screenShakeToggle.isOn = manager.UseScreenShake;
        }
    
        public void OnMasterVolumeChanged(float val) => SettingManager.Instance.SetMasterVolume(val);
    
        public void OnBgmToggleChanged(bool val) 
        { 
            SettingManager.Instance.SetBgmOn(val);
            bgmVolumeSlider.interactable = val; 
        }
        public void OnBgmVolumeChanged(float val) => SettingManager.Instance.SetBgmVolume(val);
        public void OnSfxVolumeChanged(float val) => SettingManager.Instance.SetSfxVolume(val);
        public void OnMuteToggleChanged(bool val) => SettingManager.Instance.SetMute(val);

        public void OnWindowModeToggleChanged(bool val) => SettingManager.Instance.SetScreenMode(val ? ScreenMode.Windowed : ScreenMode.FullScreen);
        public void OnResolutionChanged(int index) => SettingManager.Instance.SetResolution(index);
        public void OnQualityChanged(int index) => SettingManager.Instance.SetQuality(index);
        public void OnFrameRateChanged(int index) => SettingManager.Instance.SetFrameRate((FrameRateMode)index);

        public void OnSensitivityChanged(float val) => SettingManager.Instance.SetMouseSensitivity(val);
        public void OnScreenShakeToggleChanged(bool val) => SettingManager.Instance.SetScreenShake(val);

        // --- 제어 버튼 ---

        public void OnClickApply()
        {
            SettingManager.Instance.SaveSettings();
            gameObject.SetActive(false);
        }

        public void OnClickCancel()
        {
            SettingManager.Instance.RevertSettings();
            gameObject.SetActive(false);
        }

        public void OnClickOpenCredits() => creditsPanel.SetActive(true);
        public void OnClickCloseCredits() => creditsPanel.SetActive(false);
    }
}