using UnityEngine;
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

        private float CurrentExecuteInterval => Mathf.Max(0.1f, baseExecuteInterval / _executeSpeedMultiplier);
        private float RemainingCooldown => Mathf.Max(0f, CurrentExecuteInterval - _timer);
        public int Level { get; private set; } = 1;
        public AbilityType AbilityType => abilityType;

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
            _timer += deltaTime;
            
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
