using System;
using Managers;
using UnityEngine;
using UserInterface;

namespace Components.AbilitySystemComponent
{
    public enum AbilityType
    {
        Boomerang,
        Shield,
        Bomb,
        Dagger
    }
}

namespace Components.AbilitySystemComponent.Abilities
{
    public abstract class AbilityBase : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float baseExecuteInterval = 3f;
        [SerializeField] private AbilityType abilityType;
        [SerializeField] private Sprite abilityImage;
        [SerializeField] protected AudioClip executeClip;

        private float CurrentExecuteInterval => Mathf.Max(0.1f, baseExecuteInterval / _executeSpeedMultiplier);
        private float RemainingCooldown => Mathf.Max(0f, CurrentExecuteInterval - _timer);
        public int Level { get; private set; } = 1;
        public AbilityType AbilityType => abilityType;

        protected GameObject Owner;
        protected SoundManager SoundManagerInstance;
        
        private CoolTimePanel _coolTimePanel;
        private float _executeSpeedMultiplier = 1f;
        private float _timer;
        private bool _isEnabled;

        private void OnDisable() => _isEnabled = false;

        private void OnDestroy() => _isEnabled = false;

        public void SetCoolTimePanel(CoolTimePanel panel)
        {
            _coolTimePanel = panel;
            if (!_coolTimePanel) return;

            _coolTimePanel.SetWeaponImage(abilityImage);
            UpdateCoolTimePanel();
        }
        
        public virtual void Initialize(GameObject owner)
        {
            Owner = owner;
            _timer = CurrentExecuteInterval;
            SoundManagerInstance = SoundManager.Instance;
            _isEnabled = true;
        }
        
        public void UpdateTimer(float deltaTime)
        {
            if(!_isEnabled) return;
            
            _timer = Mathf.Min(CurrentExecuteInterval, _timer + deltaTime);
            
            UpdateCoolTimePanel();
        }

        private bool CanExecute()
        {
            return _timer >= CurrentExecuteInterval;
        }

        public virtual bool Execute()
        {
            if (!CanExecute())
            {
                return false;
            }

            return true;
        }

        protected void StartCooldown()
        {
            _timer = 0f;
            UpdateCoolTimePanel();
        }

        private void UpdateCoolTimePanel()
        {
            if (_coolTimePanel)
            {
                _coolTimePanel.SetCoolTimeText(RemainingCooldown);
            }
        }
        
        protected void AddExecuteSpeedBonus(float multiplierBonus)
        {
            _executeSpeedMultiplier = Mathf.Max(0.01f, _executeSpeedMultiplier + multiplierBonus);
        }

        public virtual void Upgrade(float amount)
        {
            if (amount <= 0f) return;

            Level++;
        }
    }
}
