using System;
using UnityEngine;

namespace Managers
{
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance { get; private set; }

        // 현재 게임에 반영된 세팅 값
        [Serializable]
        public struct SettingData
        {
            // 오디오
            public float masterVolume;
            public bool isBgmOn;
            public float bgmVolume;
            public float sfxVolume;
            public bool isMute;

            // 그래픽
            public int screenMode; // 0: FullScreen, 1: Windowed
            public int resolutionIndex;
            public int qualityIndex;
            public int frameRateLimit; // 0: 30, 1: 60, 2: Uncapped

            // 게임플레이
            public float mouseSensitivity;
            public bool useScreenShake;
        }

        public SettingData currentSettings;
        private SettingData _originalSettings; // Cancel 시 복구용 임시 저장소

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplyAllSettings();
        }

        /// <summary>
        /// UI가 켜질 때 현재 상태를 백업
        /// </summary>
        public void BackupCurrentSettings()
        {
            _originalSettings = currentSettings;
        }

        /// <summary>
        /// Cancel 버튼 클릭 시 백업본으로 원복
        /// </summary>
        public void RevertSettings()
        {
            currentSettings = _originalSettings;
            ApplyAllSettings();
        }

        /// <summary>
        /// Apply 버튼 클릭 시 PlayerPrefs에 최종 저장
        /// </summary>
        public void SaveSettings()
        {
            // 오디오
            PlayerPrefs.SetFloat("MasterVolume", currentSettings.masterVolume);
            PlayerPrefs.SetInt("IsBgmOn", currentSettings.isBgmOn ? 1 : 0);
            PlayerPrefs.SetFloat("BgmVolume", currentSettings.bgmVolume);
            PlayerPrefs.SetFloat("SfxVolume", currentSettings.sfxVolume);
            PlayerPrefs.SetInt("IsMute", currentSettings.isMute ? 1 : 0);

            // 그래픽
            PlayerPrefs.SetInt("ScreenMode", currentSettings.screenMode);
            PlayerPrefs.SetInt("ResolutionIndex", currentSettings.resolutionIndex);
            PlayerPrefs.SetInt("QualityIndex", currentSettings.qualityIndex);
            PlayerPrefs.SetInt("FrameRateLimit", currentSettings.frameRateLimit);

            // 게임플레이
            PlayerPrefs.SetFloat("MouseSensitivity", currentSettings.mouseSensitivity);
            PlayerPrefs.SetInt("UseScreenShake", currentSettings.useScreenShake ? 1 : 0);

            PlayerPrefs.Save();
            ApplyAllSettings();
        }

        /// <summary>
        /// 게임 시작 및 저장 시 값을 엔진 시스템에 로드/반영
        /// </summary>
        public void LoadSettings()
        {
            currentSettings.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            currentSettings.isBgmOn = PlayerPrefs.GetInt("IsBgmOn", 1) == 1;
            currentSettings.bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.8f);
            currentSettings.sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
            currentSettings.isMute = PlayerPrefs.GetInt("IsMute", 0) == 1;

            currentSettings.screenMode = PlayerPrefs.GetInt("ScreenMode", 0); // 기본 전체화면
            currentSettings.resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            currentSettings.qualityIndex = PlayerPrefs.GetInt("QualityIndex", 2); // 기본 High(2)
            currentSettings.frameRateLimit = PlayerPrefs.GetInt("FrameRateLimit", 2); // 기본 제한없음(2)

            currentSettings.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
            currentSettings.useScreenShake = PlayerPrefs.GetInt("UseScreenShake", 1) == 1;
        }

        /// <summary>
        /// 세팅 구조체의 값을 기반으로 실제 게임 엔진(AudioMixer, Screen 등)에 명령을 내림
        /// </summary>
        public void ApplyAllSettings()
        {
            // 오디오 적용
            // 실제 구현 시 AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20) 공식을 씁니다.
            float finalMaster = currentSettings.isMute ? 0 : currentSettings.masterVolume;
            float finalBgm = currentSettings.isBgmOn ? currentSettings.bgmVolume : 0;
        
            AudioListener.volume = finalMaster; // 가장 간단한 마스터 볼륨 제어 방법
            // TODO: SFX, BGM 오디오 소스 그룹에 finalBgm, CurrentSettings.sfxVolume 반영

            // 그래픽: 화면 모드 및 해상도 적용
            FullScreenMode mode = currentSettings.screenMode == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            if (Screen.resolutions.Length > currentSettings.resolutionIndex)
            {
                Resolution res = Screen.resolutions[currentSettings.resolutionIndex];
                Screen.SetResolution(res.width, res.height, mode);
            }

            // 그래픽 퀄리티 적용
            QualitySettings.SetQualityLevel(currentSettings.qualityIndex, true);

            // 프레임 제한 적용
            switch (currentSettings.frameRateLimit)
            {
                case 0: Application.targetFrameRate = 30; break;
                case 1: Application.targetFrameRate = 60; break;
                case 2: Application.targetFrameRate = -1; break; // 제한 없음
            }

            // 게임플레이 감도 등은 플레이어 스크립트에서 SettingManager.Instance.CurrentSettings.mouseSensitivity를 참조하도록 설계
        }
    }
}