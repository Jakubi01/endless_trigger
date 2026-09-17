using System;
using UnityEngine;

namespace Components
{
    public class PlayerLevelComponent : ComponentBase
    {
        [Header("Level")] 
        [SerializeField] private GameObject levelUpVFXPrefab;
        [SerializeField] private int requiredExperiencePerLevel = 100;

        private bool _awaitingUpgradeSelection;

        public event Action LeveledUp;
        public event Action ExperienceChanged;
        public int Level { get; private set; } = 1;

        public int CurrentExperience { get; private set; }

        public int RequiredExperiencePerLevel => requiredExperiencePerLevel;
        public float ExperiencePickupRange { get; private set; } = 2.5f;
        public GameObject LevelUpVFXPrefab => levelUpVFXPrefab;

        public void GainExperience(int amount)
        {
            if (amount <= 0) return;

            CurrentExperience += amount;
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
            if (_awaitingUpgradeSelection || CurrentExperience < requiredExperiencePerLevel) return;

            CurrentExperience -= requiredExperiencePerLevel;
            Level++;
            _awaitingUpgradeSelection = true;
            LeveledUp?.Invoke();
        }
    }
}
