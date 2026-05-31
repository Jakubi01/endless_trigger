using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GoToTitleWindow : UserInterface
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button cancelButton;

        private void Start()
        {
            if (acceptButton)
            {
                acceptButton.onClick.AddListener(OnAcceptClicked);
            }

            if (cancelButton)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }

        private void OnDestroy()
        {
            if (acceptButton)
            {
                acceptButton.onClick.RemoveListener(OnAcceptClicked);
            }

            if (cancelButton)
            {
                cancelButton.onClick.RemoveListener(OnCancelClicked);
            }
        }

        private void OnAcceptClicked()
        {
            UIManager.Instance.OnGoToTitleButtonClicked();
        }

        private void OnCancelClicked()
        {
            UIManager.Instance.OnGoToTitleWindowClosed(this);
        }
    }
}
