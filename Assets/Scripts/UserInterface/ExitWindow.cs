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

        private void Start()
        {
            InvokeRepeating(nameof(CheckButtonState), 0f, 0.1f);
        }
        
        private void CheckButtonState()
        {
            if (acceptButton != null && !acceptButton.interactable)
            {
                Debug.LogWarning("⚠️ 누군가 acceptButton을 비활성화(Disable) 했습니다!", acceptButton);
                CancelInvoke(nameof(CheckButtonState)); // 로그가 쏟아지지 않게 한 번만 출력
            }
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
