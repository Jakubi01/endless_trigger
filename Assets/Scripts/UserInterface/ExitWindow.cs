using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ExitWindow : MonoBehaviour
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button rejectButton;

        private void Start()
        {
            acceptButton.onClick.AddListener(OnAccept);
            rejectButton.onClick.AddListener(OnReject);
        }
        
        private void OnDestroy()
        {
            if(acceptButton != null)
                acceptButton.onClick.RemoveListener(OnAccept);
            
            if(rejectButton != null)
                rejectButton.onClick.RemoveListener(OnReject);
        }

        private void OnAccept()
        {
            UIManager.Instance.OnExitAccept();
        }

        private void OnReject()
        {
            UIManager.Instance.OnExitReject();
        }
    }
}
