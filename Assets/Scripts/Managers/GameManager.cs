using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int _unsavedKillCount;

        public int KillCount { get; private set; }

        public int WaveClearedCount { get; private set; }

        public int TotalKillCount => SettingManager.Instance ? SettingManager.Instance.TotalKillCount : 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
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
        }

        public void RegisterKill()
        {
            KillCount++;
            _unsavedKillCount++;
            SettingManager.Instance?.AddTotalKillCount(1);
        }

        public void RegisterWaveCleared()
        {
            WaveClearedCount++;
        }

        public void SaveRunProgress()
        {
            if (_unsavedKillCount <= 0) return;

            SettingManager.Instance?.SaveInGameProgress();
            _unsavedKillCount = 0;
        }
    }
}
