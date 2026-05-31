using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ExitWindow : UserInterface
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button cancelButton;

        private void Start()
        {
            acceptButton.onClick.AddListener(OnAccept);
            cancelButton.onClick.AddListener(OnReject);
        }
        
        private void OnDestroy()
        {
            if(acceptButton != null)
                acceptButton.onClick.RemoveListener(OnAccept);
            
            if(cancelButton != null)
                cancelButton.onClick.RemoveListener(OnReject);
        }

        private void OnAccept()
        {
            UIManager.Instance.OnExitAccept();
        }

        private void OnReject()
        {
            UIManager.Instance.OnExitCancel();
        }
    }
}
