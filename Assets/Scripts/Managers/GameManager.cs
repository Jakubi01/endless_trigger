using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int _killCount;
        private int _waveClearedCount;
        private int _unsavedKillCount;

        public int KillCount => _killCount;
        public int WaveClearedCount => _waveClearedCount;
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
            _killCount = 0;
            _waveClearedCount = 0;
            _unsavedKillCount = 0;
        }

        public void RegisterKill()
        {
            _killCount++;
            _unsavedKillCount++;
            SettingManager.Instance?.AddTotalKillCount(1);
        }

        public void RegisterWaveCleared()
        {
            _waveClearedCount++;
        }

        public void SaveRunProgress()
        {
            if (_unsavedKillCount <= 0) return;

            SettingManager.Instance?.SaveInGameProgress();
            _unsavedKillCount = 0;
        }
    }
}
