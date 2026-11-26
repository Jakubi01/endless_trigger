using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class PlayerProfilePopup : UserInterface
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerHighScore;
        [SerializeField] private TMP_Text playerTotalKillCount;
        [SerializeField] private TMP_Text totalPlayTime;

        protected override void Awake()
        {
            base.Awake();

            gameObject.SetActive(false);
        }

        public void Popup()
        {
            UpdateProfile();
            var visible = gameObject.activeSelf;
            gameObject.SetActive(!visible);
        }
        
        private void UpdateProfile()
        {
            var ingameData = SettingManager.Instance.GetInGameSaveData();
            if (ingameData == null) return;

            playerName.text = ingameData.playerName;
            playerHighScore.text = ingameData.highScore.ToString();
            playerTotalKillCount.text = ingameData.totalKillCount.ToString();
            var timeSpan = TimeSpan.FromSeconds(ingameData.totalPlayTimeSec);
            totalPlayTime.text = $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";
        }
    }
}