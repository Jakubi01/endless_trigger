using System;
using Components;
using UnityEngine;
using UserInterface;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int _unsavedKillCount;
        private bool _isRunActive;

        public int KillCount { get; private set; }

        public int WaveClearedCount { get; private set; }
        
        public float PlayTime { get; private set; }
        
        public int LevelReached
        {
            get
            {
                var sceneSpawnManager = FindAnyObjectByType<SceneSpawnManager>();
                if (!sceneSpawnManager) return 1;

                var player = sceneSpawnManager.Player;
                if (!player) return 1;

                var validComponent = player.TryGetComponent(out PlayerLevelComponent playerLevelComponent);
                return validComponent ? playerLevelComponent.Level : 1;
            }
        }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (_isRunActive)
            {
                PlayTime += Time.deltaTime;
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void BeginRun()
        {
            KillCount = 0;
            WaveClearedCount = 0;
            _unsavedKillCount = 0;
            PlayTime = 0f;
            _isRunActive = true;
        }

        public void EndRun()
        {
            _isRunActive = false;
            SaveRunProgress();
        }

        public void RegisterKill()
        {
            KillCount++;
            _unsavedKillCount++;
            SettingManager.Instance?.AddKillCount(1);
        }

        public void RegisterWaveCleared()
        {
            WaveClearedCount++;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void SaveRunProgress()
        {
            if (_unsavedKillCount <= 0) return;

            var ingameData = SettingManager.Instance.GetInGameSaveData();
            ingameData.totalPlayTimeSec += Mathf.RoundToInt(PlayTime);
            ingameData.highScore = Scoreboard.CalcScore.Item2;
            
            SettingManager.Instance?.SaveInGameProgress(ingameData);
            _unsavedKillCount = 0;
        }
    }
}
