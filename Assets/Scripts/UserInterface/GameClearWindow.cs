using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameClearWindow : UserInterface
    {
        [SerializeField] private TMP_Text roundCleared;
        [SerializeField] private TMP_Text levelRechead;
        [SerializeField] private TMP_Text enemiesSlain;
        [SerializeField] private Button goToTitle;
        [SerializeField] private Button reTry;

        private UIManager _uiManagerInstance;
        
        protected override void Awake()
        {
            base.Awake();

            _uiManagerInstance = UIManager.Instance;
            
            goToTitle?.onClick.AddListener(OnGoToTileButtonClicked);
            reTry?.onClick.AddListener(OnReTryButtonClicked);
        }

        private void OnDisable()
        {
            goToTitle?.onClick.RemoveAllListeners();
            reTry?.onClick.RemoveAllListeners();
        }

        private void OnGoToTileButtonClicked()
        {
            _uiManagerInstance?.OnGameClearWindowClosed(this);
            _uiManagerInstance?.OnGoToTitleButtonClicked();
        }

        private void OnReTryButtonClicked()
        {
            _uiManagerInstance?.OnGameClearWindowClosed(this);
            _uiManagerInstance?.OnStartButtonClicked();
        }

        public void Init()
        {
            var gameManagerInstance = GameManager.Instance;
            if(!gameManagerInstance) return;

            if (roundCleared)
            {
                roundCleared.text = gameManagerInstance.WaveClearedCount + " / 10";
            }

            if (levelRechead)
            {
                levelRechead.text = gameManagerInstance.LevelReached.ToString();
            }

            if (enemiesSlain)
            {
                enemiesSlain.text = gameManagerInstance.KillCount.ToString();
            }
        }
    }
}