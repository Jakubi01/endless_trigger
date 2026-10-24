using System;
using Character.Player;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace Components.AbilitySystemComponent
{
    public enum AbilityType
    {
        Boomerang,
        Shield,
        Bomb
    }
}

namespace Components.AbilitySystemComponent.Abilities
{
    public abstract class AbilityBase : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float baseExecuteInterval = 3f;
        [SerializeField] private AbilityType abilityType;
        [SerializeField] private Sprite abilityImage;
        
        public float CurrentExecuteInterval => Mathf.Max(0.1f, baseExecuteInterval / _executeSpeedMultiplier);
        public Sprite AbilityImage => abilityImage;
        public int Level { get; private set; } = 1;
        public AbilityType AbilityType => abilityType;
        public event Action<float> OnUpdate; 
        
        protected GameObject Owner;
        
        private float _executeSpeedMultiplier = 1f;
        private float _timer;
        private CoolTimePanel _coolTimePanel;

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
        }
        
        public void UpdateTimer(float deltaTime)
        {
            _timer = Mathf.Min(_timer + Mathf.Max(0f, deltaTime), CurrentExecuteInterval);
            OnUpdate?.Invoke(_timer);
            UpdateCoolTimePanel();
        }
        
        public bool CanExecute()
        {
            return _timer >= CurrentExecuteInterval;
        }

        public virtual bool Execute()
        {
            if (!CanExecute())
            {
                return false;
            }

            ResetTimer();
            return true;
        }

        private void ResetTimer()
        {
            _timer = 0f;
            UpdateCoolTimePanel();
        }

        private void UpdateCoolTimePanel()
        {
            if (_coolTimePanel)
            {
                _coolTimePanel.SetCoolTimeText(Mathf.Max(0f, CurrentExecuteInterval - _timer));
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
