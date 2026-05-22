using System;
using UnityEngine;

namespace Components
{
    public class PlayerLevelComponent : ComponentBase
    {
        [SerializeField] private int requiredExperiencePerLevel = 100;
        [SerializeField] private float experiencePickupRange = 2.5f;

        private int _currentExperience;
        private int _level = 1;

        public event Action LeveledUp;
        public int Level => _level;
        public int CurrentExperience => _currentExperience;
        public float ExperiencePickupRange => experiencePickupRange;

        public void GainExperience(int amount)
        {
            if (amount <= 0) return;

            _currentExperience += amount;
            while (_currentExperience >= requiredExperiencePerLevel)
            {
                _currentExperience -= requiredExperiencePerLevel;
                _level++;
                LeveledUp?.Invoke();
            }
        }
    }
}
