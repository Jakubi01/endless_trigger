using Character.Player;
using Components;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameHUD : UserInterface
    {
        [SerializeField] private Slider hpBarSlider;
        [SerializeField] private TMP_Text hpBarText;
        [SerializeField] private Slider expBarSlider;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private StatusWindow statusWindow;

        private PlayerCharacter _player;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;
        private EnemySpawner _enemySpawner;
        private int _lastTimerMode = -1;
        private int _lastTimerSecond = -1;

        protected override void Awake()
        {
            base.Awake();
            
            Canvas.ForceUpdateCanvases();
        }
        
        public void Initialize(PlayerCharacter player, EnemySpawner enemySpawner)
        {
            UnsubscribeStatEvents();

            _player = player;
            _enemySpawner = enemySpawner;
            CachePlayerReferences();
            SubscribeStatEvents();
            RefreshStats();
        }
        
        private void Update()
        {
            RefreshTimer();
        }

        private void RefreshTimer()
        {
            if (!timerText || !_enemySpawner) return;

            int mode;
            float time;

            if (_enemySpawner.TimeUntilGameStart > 0f)
            {
                mode = 0;
                time = _enemySpawner.TimeUntilGameStart;
            }
            else if (_enemySpawner.NextRoundCountdown > 0f)
            {
                mode = 1;
                time = _enemySpawner.NextRoundCountdown;
            }
            else
            {
                mode = 2;
                time = _enemySpawner.CurrentWaveRemainingTime;
            }

            var second = Mathf.CeilToInt(time);
            if (mode == _lastTimerMode && second == _lastTimerSecond) return;

            _lastTimerMode = mode;
            _lastTimerSecond = second;
            timerText.text = mode switch
            {
                0 => $"{second}",
                1 => $"{second}",
                _ => $"{_enemySpawner.CurrentRound} | {second}"
            };
        }

        private void OnDestroy()
        {
            UnsubscribeStatEvents();
        }

        private void CachePlayerReferences()
        {
            if (!_player) return;

            _healthComponent = _player.GetComponent<HealthComponent>();
            _levelComponent = _player.GetComponent<PlayerLevelComponent>();
        }

        private void SubscribeStatEvents()
        {
            if (_healthComponent)
            {
                _healthComponent.HealthChanged += RefreshHealth;
            }

            if (_levelComponent)
            {
                _levelComponent.ExperienceChanged += RefreshExperience;
            }
        }

        private void UnsubscribeStatEvents()
        {
            if (_healthComponent)
            {
                _healthComponent.HealthChanged -= RefreshHealth;
            }

            if (_levelComponent)
            {
                _levelComponent.ExperienceChanged -= RefreshExperience;
            }
        }

        private void RefreshStats()
        {
            RefreshHealth();
            RefreshExperience();
        }

        private void RefreshHealth()
        {
            if (!hpBarSlider || !hpBarText || !_healthComponent) return;

            float maxHealth = _healthComponent.MaxHealth;
            float currentHealth = _healthComponent.CurrentHealth;
            
            hpBarSlider.minValue = 0f;
            hpBarSlider.maxValue = maxHealth;
            hpBarSlider.value = currentHealth;
            hpBarText.text = currentHealth + " / " + maxHealth;
        }

        private void RefreshExperience()
        {
            if (_levelComponent)
            {
                if (expBarSlider)
                {
                    expBarSlider.minValue = 0f;
                    expBarSlider.maxValue = Mathf.Max(1, _levelComponent.RequiredExperiencePerLevel);
                    expBarSlider.value = _levelComponent.CurrentExperience;
                }

                if (levelText)
                {
                    levelText.text = $"Lv {_levelComponent.Level}";
                }
            }
            else if (levelText)
            {
                levelText.text = "Lv 0";
            }
        }
    }
}
