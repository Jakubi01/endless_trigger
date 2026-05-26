using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class PauseWindow : UserInterface
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button gotoTitleButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            if(continueButton)
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            
            if(settingButton)
                settingButton.onClick.AddListener(OnSettingsButtonClicked);
            
            if(gotoTitleButton)
                gotoTitleButton.onClick.AddListener(OnGoToTitleButtonClicked);
            
            if(exitButton)
                exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnDestroy()
        {
            if(continueButton)
                continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            
            if(settingButton)
                settingButton.onClick.RemoveListener(OnSettingsButtonClicked);
            
            if(gotoTitleButton)
                gotoTitleButton.onClick.RemoveListener(OnGoToTitleButtonClicked);
            
            if(exitButton)
                exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        private void OnContinueButtonClicked()
        {
            UIManager.Instance.OnContinueButtonClicked();
        }

        private void OnSettingsButtonClicked()
        {
            UIManager.Instance.OnSettingsButtonClicked();
        }

        private void OnGoToTitleButtonClicked()
        {
            UIManager.Instance.OnGoToTitleButtonClicked();
        }

        private void OnExitButtonClicked()
        {
            UIManager.Instance.OnExitButtonClicked();
        }
    }
}