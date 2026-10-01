using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Settings
{
    public class SoundSettingWindow : UserInterface, ISettingWindow
    {
        [Header("Audio UI")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle bgmToggle;
        [SerializeField] private Text   bgmToggleText;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Text   muteToggleText;

        public WindowType Type { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            InitSlider();
            InitToggleValue();
            RegisterUIEvents();

            Type = WindowType.Sound;
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
        }

        private void InitToggleValue()
        {
            SetToggleValue(bgmToggle, true);
            SetToggleText(bgmToggle, bgmToggleText);

            SetToggleValue(muteToggle, false);
            SetToggleText(muteToggle, muteToggleText);
        }

        private void RegisterUIEvents()
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);
            bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
        }

        private void UnRegisterUIEvents()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            bgmToggle.onValueChanged.RemoveListener(OnBgmToggleChanged);
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            muteToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);
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
        }
        
        private void OnBgmToggleChanged(bool val)
        {
            SettingManager.Instance.SetBgmOn(val);
            SetToggleText(bgmToggle, bgmToggleText);
            bgmVolumeSlider.interactable = val;
        }
        
        private void OnMuteToggleChanged(bool val)
        {
            SettingManager.Instance.SetMute(val);
            SetToggleText(muteToggle, muteToggleText);
        }
        
        private void OnMasterVolumeChanged(float val) => SettingManager.Instance.SetMasterVolume(val);
        private void OnBgmVolumeChanged(float val) => SettingManager.Instance.SetBgmVolume(val);
        private void OnSfxVolumeChanged(float val) => SettingManager.Instance.SetSfxVolume(val);

    }
}