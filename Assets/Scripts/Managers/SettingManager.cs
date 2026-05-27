using System;
using Types;
using UnityEngine;
using SaveSystem; // JSON 세이브 시스템 네임스페이스 참조

namespace Managers
{
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance { get; private set; }
        
        // 고정된 독립 파일 이름 정의
        private const string SETTINGS_FILE_NAME = "Settings.json";
        private const string INGAME_SAVE_FILE_NAME = "SaveGame.json";

        // 런타임 메모리 데이터 홀더
        private SettingData _currentSettings;
        private SettingData _originalSettings;
        private InGameSaveData _inGameSaveData;
        
        // --- 외부 노출용 프로퍼티 ---
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

        // 인게임 플레이 진행 데이터 접근용
        public InGameSaveData InGameSave => _inGameSaveData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                LoadAllDataPool();
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
        /// 모든 파일 독립 로드
        /// </summary>
        private void LoadAllDataPool()
        {
            _currentSettings = SaveSystem.SaveSystem.Load<SettingData>(SETTINGS_FILE_NAME);
            _inGameSaveData = SaveSystem.SaveSystem.Load<InGameSaveData>(INGAME_SAVE_FILE_NAME);
        }

        /// <summary>
        /// UI 오픈 시 현재 런타임 데이터를 백업
        /// </summary>
        public void BackupCurrentSettings()
        {
            _originalSettings = _currentSettings.Clone();
        }

        /// <summary>
        ///Cancel 클릭 시 백업본에서 복구 후 전역 반영
        /// </summary>
        public void RevertSettings()
        {
            _currentSettings = _originalSettings.Clone();
            ApplyAllSettings();
        }
        
        /// <summary>
        /// Apply 클릭 시 세팅 파일만 단독 JSON 저장 및 그래픽 옵션 최종 반영
        /// </summary>
        public void SaveSettings()
        {
            SaveSystem.SaveSystem.Save(SETTINGS_FILE_NAME, _currentSettings);
            
            // 디스플레이 갱신은 최종 확정 시에만 수행
            ApplyGraphicsSettings();
        }

        /// <summary>
        /// 인게임 세이브 전용 제어부
        ///체력 변화, 골드 획득, 스테이지 클리어 시점에 단독 호출
        /// </summary>
        public void SaveInGameProgress()
        {
            SaveSystem.SaveSystem.Save(INGAME_SAVE_FILE_NAME, _inGameSaveData);
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
        
        // 그래픽 세팅은 조작 단계에서 프리즈/깜빡임 방지를 위해 ApplyGraphicsSettings를 실시간 호출하지 않음
        public void SetScreenMode(ScreenMode mode)   => _currentSettings.screenMode = mode;
        public void SetResolution(int index)         => _currentSettings.resolutionIndex = index;
        public void SetQuality(int index)            => _currentSettings.qualityIndex = index;
        public void SetFrameRate(FrameRateMode mode) => _currentSettings.frameRateMode = mode;
        
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

            // TODO: 사운드 매니저 연동 시 finalBgm(_currentSettings.isBgmOn 반영) 및 sfxVolume 전달
        }

        public void ApplyGraphicsSettings()
        {
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
                case FrameRateMode.FPS30:    Application.targetFrameRate = 30;   break;
                case FrameRateMode.FPS60:    Application.targetFrameRate = 60;   break;
                case FrameRateMode.FPS120:   Application.targetFrameRate = 120;  break;
                case FrameRateMode.FPS240:   Application.targetFrameRate = 240;  break;
                case FrameRateMode.FPS300:   Application.targetFrameRate = 300;  break;
                case FrameRateMode.Uncapped: Application.targetFrameRate = -1;   break;
            }
        }

        public void ApplyGameplaySettings()
        {
            // 마우스 감도 인게임 카메라에 전달 및 화면 진동 켜고 끄기 분기점 제어
        }
    }
}