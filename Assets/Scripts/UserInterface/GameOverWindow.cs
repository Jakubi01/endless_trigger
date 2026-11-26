using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameOverWindow : UserInterface
    {
        [SerializeField] private Button reTryButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TMP_Text waveClearedText;
        [SerializeField] private TMP_Text killCountText;
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            RefreshResultTexts();

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

        private void RefreshResultTexts()
        {
            var gameManager = GameManager.Instance;
            var waveClearedCount = gameManager ? gameManager.WaveClearedCount : 0;
            var killCount = gameManager ? gameManager.KillCount : 0;

            if (waveClearedText)
            {
                waveClearedText.SetText("{0}", waveClearedCount);
            }

            if (killCountText)
            {
                killCountText.SetText("{0}", killCount);
            }

            if (scoreText)
            {
                scoreText.SetText("{0}", Scoreboard.CalcScore.Item1);
            }

            gameManager?.SaveRunProgress();
        }
    }
}
