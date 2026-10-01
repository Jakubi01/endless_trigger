using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Settings
{
    public interface ISettingWindow
    {
        WindowType Type { get; }
    }
    
    public enum WindowType
    {
        Graphic,
        Sound,
        Gameplay
    }
    
    public class SettingWindow : UserInterface
    {
        [SerializeField] private GraphicSettingWindow graphicSettingWindow;
        [SerializeField] private SoundSettingWindow soundSettingWindow;
        [SerializeField] private GameplaySettingWindow gameplaySettingWindow;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button displayButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button gameplayButton;
        
        protected override void Awake()
        {
            base.Awake();

            // set default window
            graphicSettingWindow.gameObject.SetActive(true);
            soundSettingWindow.gameObject.SetActive(false);
            gameplaySettingWindow.gameObject.SetActive(false);
            SetColorAlpha(WindowType.Graphic);
        }

        private void OnEnable()
        {
            // register listener
            applyButton.onClick.AddListener(OnClickApply);
            rejectButton.onClick.AddListener(OnClickCancel);
            displayButton.onClick.AddListener(() => OpenPanel(graphicSettingWindow.gameObject));
            soundButton.onClick.AddListener(() => OpenPanel(soundSettingWindow.gameObject));
            gameplayButton.onClick.AddListener(() => OpenPanel(gameplaySettingWindow.gameObject));
        }

        private void OnDestroy()
        {
            // unregister listener
            applyButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();
            displayButton.onClick.RemoveAllListeners();
            soundButton.onClick.RemoveAllListeners();
            gameplayButton.onClick.RemoveAllListeners();
        }

        public void OnClickApply()
        {
            SettingManager.Instance.SaveSettings();
            CloseWindow();
        }

        private void OpenPanel(GameObject target)
        {
            graphicSettingWindow.gameObject.SetActive(false);
            soundSettingWindow.gameObject.SetActive(false);
            gameplaySettingWindow.gameObject.SetActive(false);
            
            target.SetActive(true);
            if(target.TryGetComponent(out ISettingWindow window))
                SetColorAlpha(window.Type);
        }

        private void SetColorAlpha(WindowType type)
        {
            var display = displayButton.image.color;
            var sound = soundButton.image.color;
            var gameplay = gameplayButton.image.color;
            
            switch (type)
            {
                case WindowType.Graphic:
                    display.a = 1f;
                    sound.a = .2f;
                    gameplay.a = .2f;
                    
                    displayButton.image.color = display;
                    soundButton.image.color = sound;
                    gameplayButton.image.color = gameplay;
                    break;
                
                case WindowType.Sound:
                    display.a = .2f;
                    sound.a = 1f;
                    gameplay.a = .2f;
                    
                    displayButton.image.color = display;
                    soundButton.image.color = sound;
                    gameplayButton.image.color = gameplay;
                    break;
                
                case WindowType.Gameplay:
                    display.a = .2f;
                    sound.a = .2f;
                    gameplay.a = 1f;
                    
                    displayButton.image.color = display;
                    soundButton.image.color = sound;
                    gameplayButton.image.color = gameplay;
                    break;
            }
        }

        public void OnClickCancel()
        {
            SettingManager.Instance.RevertSettings();
            CloseWindow();
        }
        
        private void CloseWindow()
        {
            if (UIManager.Instance)
            {
                UIManager.Instance.OnSettingWindowClosed(this);
                return;
            }

            gameObject.SetActive(false);
        }
    }
}
