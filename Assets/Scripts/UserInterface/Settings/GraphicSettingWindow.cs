using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Settings
{
    public class GraphicSettingWindow : UserInterface, ISettingWindow
    {
        [Header("Graphics UI")]
        [SerializeField] private Toggle windowModeToggle;
        [SerializeField] private Text   windowModeToggleText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        
        private readonly List<Resolution> _systemResolutions = new();
        
        public WindowType Type { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            InitToggleValue();
            InitFrameRateDropdown();
            InitQualityDropdown();
            InitResolutionDropdown();
            RegisterUIEvents();

            Type = WindowType.Graphic;
        }

        private void OnEnable()
        {
            SettingManager.Instance.BackupCurrentSettings();
            UpdateUIFromManager();
        }

        private void OnDestroy()
        {
            UnRegisterUIEvents();
        }

        private void InitToggleValue()
        {
            var isWindowMode = SettingManager.Instance.CurrentScreenMode == ScreenMode.Windowed;
            SetToggleValue(windowModeToggle, isWindowMode);
            SetToggleText(windowModeToggle, windowModeToggleText);
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

        private void RegisterUIEvents()
        {
            windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        }

        private void UnRegisterUIEvents()
        {
            windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);
            frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateChanged);
        }

        private void UpdateUIFromManager()
        {
            var manager = SettingManager.Instance;

            windowModeToggle.SetIsOnWithoutNotify(manager.CurrentScreenMode == ScreenMode.Windowed);
            resolutionDropdown.SetValueWithoutNotify(GetValidResolutionIndex(manager.ResolutionIndex));
            qualityDropdown.SetValueWithoutNotify(GetValidQualityIndex(manager.QualityIndex));
            frameRateDropdown.SetValueWithoutNotify(GetValidFrameRateIndex((int)manager.CurrentFrameRateMode));
            resolutionDropdown.RefreshShownValue();
            qualityDropdown.RefreshShownValue();
            frameRateDropdown.RefreshShownValue();
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