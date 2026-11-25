using Managers;
using SaveSystem;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace UserInterface
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private TMP_Text newScoreText;
        
        private InGameSaveData _inGameSaveData;
        
        private void Awake()
        {
            _inGameSaveData = SettingManager.Instance?.GetInGameSaveData();
        }

        public void SetText()
        {
            var pair = CalcScore();
            newScoreText.text = pair.Item1.ToString();
            highScoreText.text = pair.Item2.ToString();
        }
        
        private (int, int) CalcScore()
        {
            if (_inGameSaveData == null) return (0, 0);

            var killCount = GameManager.Instance?.KillCount;
            var playTime = GameManager.Instance?.PlayTime;
            
            var newScore = /* Calc Score here */ 0;
            var highScore = _inGameSaveData.highScore;
            return (newScore, highScore);
        }

        public void SaveScore()
        {
            SettingManager.Instance?.SaveInGameProgress(_inGameSaveData);
        }
    }
}