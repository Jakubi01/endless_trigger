using System;
using Managers;
using UnityEngine;

namespace Components
{
    public class PlayerLevelComponent : ComponentBase
    {
        [Header("Level")] 
        [SerializeField] private GameObject levelUpVFXPrefab;

        private bool _awaitingUpgradeSelection;
        private const float NextExperienceMult = 1.15f;

        public event Action LeveledUp;
        public event Action ExperienceChanged;
        public int Level { get; private set; } = 1;

        public int CurrentExperience { get; private set; }

        public int RequiredExperiencePerLevel { get; private set; }
        public float ExperiencePickupRange { get; private set; } = 2.5f;
        public GameObject LevelUpVFXPrefab => levelUpVFXPrefab;

        protected override void Awake()
        {
            base.Awake();

            RequiredExperiencePerLevel = 100;
        }
        
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
            if (_awaitingUpgradeSelection || CurrentExperience < RequiredExperiencePerLevel) return;

            CurrentExperience -= RequiredExperiencePerLevel;
            RequiredExperiencePerLevel = Mathf.RoundToInt(RequiredExperiencePerLevel * NextExperienceMult);
            Level++;
            _awaitingUpgradeSelection = true;
            LeveledUp?.Invoke();
            SoundManager.Instance.Play(SfxType.LevelUp);
        }
    }
}
