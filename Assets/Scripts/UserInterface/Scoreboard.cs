using Managers;
using UnityEngine;

namespace UserInterface
{
    public static class Scoreboard
    {
        private const int ScorePerKill = 50;
        private const int ScorePerRound = 10000;
        private const int ScorePerSecond = 100;

        /// <returns> new score, high score </returns>
        public static (int, int) CalcScore
        {
            get
            {
                var inGameSaveData = SettingManager.Instance.GetInGameSaveData();
                if (inGameSaveData == null) return (0, 0);

                var killCount = GameManager.Instance.KillCount;
                var reachedRound = GameManager.Instance.WaveClearedCount;
                var playTime = GameManager.Instance.PlayTime;
                
                var newScore = Mathf.RoundToInt(killCount * ScorePerKill + reachedRound * ScorePerRound + playTime * ScorePerSecond);
                inGameSaveData.highScore = newScore >= inGameSaveData.highScore ? newScore : inGameSaveData.highScore;
                SettingManager.Instance?.SaveInGameProgress(inGameSaveData);
                
                return (newScore, inGameSaveData.highScore);
            }
        }

        public static int GetHighScore() => SettingManager.Instance.GetInGameSaveData().highScore;
    }
}