using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameOverWindow : UserInterface
    {
        [SerializeField] private Button reTryButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            if (reTryButton)
            {
                reTryButton.onClick.AddListener(OnReTryButtonClicked);
            }

            if (titleButton)
            {
                titleButton.onClick.AddListener(OnTitleButtonClicked);
            }

            if (exitButton)
            {
                exitButton.onClick.AddListener(OnExitButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (reTryButton)
            {
                reTryButton.onClick.RemoveListener(OnReTryButtonClicked);
            }

            if (titleButton)
            {
                titleButton.onClick.RemoveListener(OnTitleButtonClicked);
            }

            if (exitButton)
            {
                exitButton.onClick.RemoveListener(OnExitButtonClicked);
            }
        }

        private void OnReTryButtonClicked()
        {
            UIManager.Instance.OnGameOverWindowClosed(this);
            UIManager.Instance.OnStartButtonClicked();
        }

        private void OnTitleButtonClicked()
        {
            UIManager.Instance.ShowGoToTitleWindow();
        }

        private void OnExitButtonClicked()
        {
            UIManager.Instance.OnExitButtonClicked();
        }
    }
}
