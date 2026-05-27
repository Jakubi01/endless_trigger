using System;
using Types;
using UnityEngine;

namespace Managers
{
    [Serializable]
    public class SettingData
    {
        // 오디오
        public float masterVolume = 1.0f;
        public bool isBgmOn = true;
        public float bgmVolume = 0.8f;
        public float sfxVolume = 0.8f;
        public bool isMute = false;

        // 그래픽
        public ScreenMode screenMode = ScreenMode.FullScreen;
        public int resolutionIndex = -1;
        public int qualityIndex = 2;
        public FrameRateMode frameRateMode = FrameRateMode.Uncapped;

        // 게임플레이
        public float mouseSensitivity = 1.0f;
        public bool useScreenShake = true;

        // Cancel 시 원복을 위한 깊은 복사(Deep Copy) 메서드
        public SettingData Clone()
        {
            return new SettingData
            {
                masterVolume = this.masterVolume,
                isBgmOn = this.isBgmOn,
                bgmVolume = this.bgmVolume,
                sfxVolume = this.sfxVolume,
                isMute = this.isMute,

                screenMode = this.screenMode,
                resolutionIndex = this.resolutionIndex,
                qualityIndex = this.qualityIndex,
                frameRateMode = this.frameRateMode,

                mouseSensitivity = this.mouseSensitivity,
                useScreenShake = this.useScreenShake
            };
        }
    }
    
    public class SettingManager : MonoBehaviour
        {
            public static SettingManager Instance { get; private set; }
            
            private const string KEY_MASTER_VOLUME = "Setting_MasterVolume";
            private const string KEY_IS_BGM_ON = "Setting_IsBgmOn";
            private const string KEY_BGM_VOLUME = "Setting_BgmVolume";
            private const string KEY_SFX_VOLUME = "Setting_SfxVolume";
            private const string KEY_IS_MUTE = "Setting_IsMute";
            private const string KEY_SCREEN_MODE = "Setting_ScreenMode";
            private const string KEY_RESOLUTION_IDX = "Setting_ResolutionIndex";
            private const string KEY_QUALITY_IDX = "Setting_QualityIndex";
            private const string KEY_FRAMERATE_MODE = "Setting_FrameRateMode";
            private const string KEY_SENSITIVITY = "Setting_MouseSensitivity";
            private const string KEY_SCREEN_SHAKE = "Setting_UseScreenShake";

            private SettingData _currentSettings;
            private SettingData _originalSettings;
            
            public float MasterVolume => _currentSettings.masterVolume;
            public bool IsBgmOn => _currentSettings.isBgmOn;
            public float BgmVolume => _currentSettings.bgmVolume;
            public float SfxVolume => _currentSettings.sfxVolume;
            public bool IsMute => _currentSettings.isMute;
            public ScreenMode CurrentScreenMode => _currentSettings.screenMode;
            public int ResolutionIndex => _currentSettings.resolutionIndex;
            public int QualityIndex => _currentSettings.qualityIndex;
            public FrameRateMode CurrentFrameRateMode => _currentSettings.frameRateMode;
            public float MouseSensitivity => _currentSettings.mouseSensitivity;
            public bool UseScreenShake => _currentSettings.useScreenShake;

            private void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);

                    _currentSettings = new SettingData(); // 클래스 인스턴스 생성
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

            // UI 오픈 시 현재 클래스 데이터를 백업 (Clone 이용)
            public void BackupCurrentSettings()
            {
                _originalSettings = _currentSettings.Clone();
            }

            // Cancel 클릭 시 백업본에서 복구 후 전역 반영
            public void RevertSettings()
            {
                _currentSettings = _originalSettings.Clone();
                ApplyAllSettings();
            }

            // Apply 클릭 시 디스크 저장 및 그래픽 옵션 최종 반영
            public void SaveSettings()
            {
                PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, _currentSettings.masterVolume);
                PlayerPrefs.SetInt(KEY_IS_BGM_ON, _currentSettings.isBgmOn ? 1 : 0);
                PlayerPrefs.SetFloat(KEY_BGM_VOLUME, _currentSettings.bgmVolume);
                PlayerPrefs.SetFloat(KEY_SFX_VOLUME, _currentSettings.sfxVolume);
                PlayerPrefs.SetInt(KEY_IS_MUTE, _currentSettings.isMute ? 1 : 0);

                PlayerPrefs.SetInt(KEY_SCREEN_MODE, (int)_currentSettings.screenMode);
                PlayerPrefs.SetInt(KEY_RESOLUTION_IDX, _currentSettings.resolutionIndex);
                PlayerPrefs.SetInt(KEY_QUALITY_IDX, _currentSettings.qualityIndex);
                PlayerPrefs.SetInt(KEY_FRAMERATE_MODE, (int)_currentSettings.frameRateMode);

                PlayerPrefs.SetFloat(KEY_SENSITIVITY, _currentSettings.mouseSensitivity);
                PlayerPrefs.SetInt(KEY_SCREEN_SHAKE, _currentSettings.useScreenShake ? 1 : 0);

                PlayerPrefs.Save();
                
                ApplyGraphicsSettings();
            }

            private void LoadSettings()
            {
                _currentSettings.masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, 1.0f);
                _currentSettings.isBgmOn = PlayerPrefs.GetInt(KEY_IS_BGM_ON, 1) == 1;
                _currentSettings.bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 0.8f);
                _currentSettings.sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 0.8f);
                _currentSettings.isMute = PlayerPrefs.GetInt(KEY_IS_MUTE, 0) == 1;

                _currentSettings.screenMode = (ScreenMode)PlayerPrefs.GetInt(KEY_SCREEN_MODE, 0);
                _currentSettings.resolutionIndex = PlayerPrefs.GetInt(KEY_RESOLUTION_IDX, -1);
                _currentSettings.qualityIndex = PlayerPrefs.GetInt(KEY_QUALITY_IDX, 2);
                _currentSettings.frameRateMode = (FrameRateMode)PlayerPrefs.GetInt(KEY_FRAMERATE_MODE, 2);

                _currentSettings.mouseSensitivity = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 1.0f);
                _currentSettings.useScreenShake = PlayerPrefs.GetInt(KEY_SCREEN_SHAKE, 1) == 1;
            }

            // --- Setter APIs ---
            
            public void SetMasterVolume(float volume)
            {
                _currentSettings.masterVolume = Mathf.Clamp01(volume);
                ApplyAudioSettings();
            }

            public void SetBgmOn(bool isOn)
            {
                _currentSettings.isBgmOn = isOn;
                ApplyAudioSettings();
            }

            public void SetBgmVolume(float volume)
            {
                _currentSettings.bgmVolume = Mathf.Clamp01(volume);
                ApplyAudioSettings();
            }

            public void SetSfxVolume(float volume)
            {
                _currentSettings.sfxVolume = Mathf.Clamp01(volume);
                ApplyAudioSettings();
            }

            public void SetMute(bool isMute)
            {
                _currentSettings.isMute = isMute;
                ApplyAudioSettings();
            }
            
            public void SetScreenMode(ScreenMode mode)
            {
                _currentSettings.screenMode = mode;
            }

            public void SetResolution(int index)
            {
                _currentSettings.resolutionIndex = index;
            }

            public void SetQuality(int index)
            {
                _currentSettings.qualityIndex = index;
            }

            public void SetFrameRate(FrameRateMode mode)
            {
                _currentSettings.frameRateMode = mode;
            }
            
            public void SetMouseSensitivity(float sensitivity)
            {
                _currentSettings.mouseSensitivity = sensitivity;
                ApplyGameplaySettings();
            }

            public void SetScreenShake(bool useShake)
            {
                _currentSettings.useScreenShake = useShake;
                ApplyGameplaySettings();
            }


            // --- Apply 세부 분리 구현 ---

            public void ApplyAllSettings()
            {
                ApplyAudioSettings();
                ApplyGraphicsSettings();
                ApplyGameplaySettings();
            }

            public void ApplyAudioSettings()
            {
                float finalMaster = _currentSettings.isMute ? 0 : _currentSettings.masterVolume;
                AudioListener.volume = finalMaster;

                // TODO: 사운드 매니저 연동 시 finalBgm 및 sfxVolume 전달
            }

            public void ApplyGraphicsSettings()
            {
                // 디스플레이 재초기화 위험이 있는 구간이므로 Apply 시점에 단 한 번만 안전하게 호출
                FullScreenMode mode = _currentSettings.screenMode == ScreenMode.FullScreen
                    ? FullScreenMode.FullScreenWindow
                    : FullScreenMode.Windowed;

                if (Screen.resolutions.Length > _currentSettings.resolutionIndex && _currentSettings.resolutionIndex >= 0)
                {
                    Resolution res = Screen.resolutions[_currentSettings.resolutionIndex];
                    Screen.SetResolution(res.width, res.height, mode);
                }

                QualitySettings.SetQualityLevel(_currentSettings.qualityIndex, true);

                switch (_currentSettings.frameRateMode)
                {
                    case FrameRateMode.FPS30: Application.targetFrameRate = 30; break;
                    case FrameRateMode.FPS60: Application.targetFrameRate = 60; break;
                    case FrameRateMode.Uncapped: Application.targetFrameRate = -1; break;
                }
            }

            public void ApplyGameplaySettings()
            {
                // 마우스 감도 및 화면 진동 컴포넌트 실시간 동기화
            }
        }
}