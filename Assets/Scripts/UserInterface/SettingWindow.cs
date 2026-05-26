using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class SettingWindow : UserInterface
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

        private List<Resolution> _systemResolutions = new();
        
        private void Awake()
        {
            InitResolutionDropdown();
        }
        
        private void OnEnable()
        {
            // UI가 켜질 때 현재 설정을 복사(백업)하고 화면 갱신
            SettingManager.Instance.BackupCurrentSettings();
            UpdateUIFromManager();
        }
        
        /// <summary>
        /// 시스템 해상도 목록을 가져와 드롭다운에 빌드합니다.
        /// </summary>
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
            
            int savedResIndex = SettingManager.Instance.currentSettings.resolutionIndex;
            if (savedResIndex >= 0 && savedResIndex < _systemResolutions.Count)
            {
                resolutionDropdown.value = savedResIndex;
            }
            else
            {
                resolutionDropdown.value = currentResIndex;
                SettingManager.Instance.currentSettings.resolutionIndex = currentResIndex;
            }

            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// SettingManager의 데이터를 UI 컴포넌트들에 시각적으로 동기화
        /// </summary>
        private void UpdateUIFromManager()
        {
            var data = SettingManager.Instance.currentSettings;

            // 오디오
            masterVolumeSlider.value = data.masterVolume;
            bgmToggle.isOn = data.isBgmOn;
            bgmVolumeSlider.value = data.bgmVolume;
            bgmVolumeSlider.interactable = data.isBgmOn; // BGM 꺼져있으면 슬라이더 비활성화
            sfxVolumeSlider.value = data.sfxVolume;
            muteToggle.isOn = data.isMute;

            // 그래픽
            windowModeToggle.isOn = (data.screenMode == 1);
            resolutionDropdown.value = data.resolutionIndex;
            qualityDropdown.value = data.qualityIndex;
            frameRateDropdown.value = data.frameRateLimit;

            // 게임플레이
            sensitivitySlider.value = data.mouseSensitivity;
            screenShakeToggle.isOn = data.useScreenShake;
        }
         
        // ----- UI 값들이 변경될 때 Manager의 임시 데이터를 실시간으로 업데이트(OnValueChanged에 연결) -----
        
        public void OnMasterVolumeChanged(float val) { SettingManager.Instance.currentSettings.masterVolume = val; }
    
        public void OnBgmToggleChanged(bool val) 
        { 
            SettingManager.Instance.currentSettings.isBgmOn = val;
            bgmVolumeSlider.interactable = val; // 토글 상태에 따라 슬라이더 활성/비활성
        }
        
        public void OnBgmVolumeChanged(float val) { SettingManager.Instance.currentSettings.bgmVolume = val; }
        public void OnSfxVolumeChanged(float val) { SettingManager.Instance.currentSettings.sfxVolume = val; }
        public void OnMuteToggleChanged(bool val) { SettingManager.Instance.currentSettings.isMute = val; }

        public void OnWindowModeToggleChanged(bool val) { SettingManager.Instance.currentSettings.screenMode = val ? 1 : 0; }
        public void OnResolutionChanged(int index) { SettingManager.Instance.currentSettings.resolutionIndex = index; }
        public void OnQualityChanged(int index) { SettingManager.Instance.currentSettings.qualityIndex = index; }
        public void OnFrameRateChanged(int index) { SettingManager.Instance.currentSettings.frameRateLimit = index; }

        public void OnSensitivityChanged(float val) { SettingManager.Instance.currentSettings.mouseSensitivity = val; }
        public void OnScreenShakeToggleChanged(bool val) { SettingManager.Instance.currentSettings.useScreenShake = val; }
        
        public void OnClickApply()
        {
            SettingManager.Instance.SaveSettings();
            gameObject.SetActive(false); // 세팅 창 닫기
        }

        public void OnClickCancel()
        {
            SettingManager.Instance.RevertSettings();
            gameObject.SetActive(false); // 세팅 창 닫기
        }

        public void OnClickOpenCredits() { creditsPanel.SetActive(true); }
        public void OnClickCloseCredits() { creditsPanel.SetActive(false); }
    }
}