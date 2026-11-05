using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ExitWindow : UserInterface
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button cancelButton;

        protected override void Awake()
        {
            base.Awake();
            
            acceptButton.onClick.AddListener(OnAccept);
            cancelButton.onClick.AddListener(OnReject);
        }
        
        private void OnDestroy()
        {
            if(acceptButton)
                acceptButton.onClick.RemoveListener(OnAccept);
            
            if(cancelButton)
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
