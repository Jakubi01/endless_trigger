using System;
using UnityEngine;

namespace Components
{
    public class PlayerLevelComponent : ComponentBase
    {
        [Header("Level")] 
        [SerializeField] private GameObject levelUpVFXPrefab;
        [SerializeField] private int requiredExperiencePerLevel = 100;
        [SerializeField] private float experiencePickupRange = 2.5f;

        private int _currentExperience;
        private int _level = 1;
        private bool _awaitingUpgradeSelection;

        public event Action LeveledUp;
        public event Action ExperienceChanged;
        public int Level => _level;
        public int CurrentExperience => _currentExperience;
        public int RequiredExperiencePerLevel => requiredExperiencePerLevel;
        public float ExperiencePickupRange => experiencePickupRange;
        public GameObject LevelUpVFXPrefab => levelUpVFXPrefab;

        public void GainExperience(int amount)
        {
            if (amount <= 0) return;

            _currentExperience += amount;
            TryStartNextLevelUp();
            ExperienceChanged?.Invoke();
        }

        /// <summary>
        /// 선택 창이 닫힌 뒤 남은 경험치로 다음 레벨업을 처리합니다.
        /// </summary>
        public void CompleteUpgradeSelection()
        {
            if (!_awaitingUpgradeSelection) return;

            _awaitingUpgradeSelection = false;
            TryStartNextLevelUp();
            ExperienceChanged?.Invoke();
        }

        private void TryStartNextLevelUp()
        {
            if (_awaitingUpgradeSelection || _currentExperience < requiredExperiencePerLevel) return;

            _currentExperience -= requiredExperiencePerLevel;
            _level++;
            _awaitingUpgradeSelection = true;
            LeveledUp?.Invoke();
        }
    }
}
