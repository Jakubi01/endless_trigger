using UnityEngine;

namespace Components.AbilitySystem
{
    public enum AbilityType
    {
        Boomerang,
        Shield,
        Bomb
    }
}

namespace Components.AbilitySystem.Abilities
{
    public abstract class AbilityBase : ScriptableObject
    {
        protected GameObject Owner;
        [SerializeField, Min(0.1f)] private float baseExecuteInterval = 3f;
        private float _executeSpeedMultiplier = 1f;
        private float _timer;
        public int Level { get; private set; } = 1;
        
        public float CurrentExecuteInterval => Mathf.Max(0.1f, baseExecuteInterval / _executeSpeedMultiplier);
        [SerializeField] private AbilityType abilityType;
        public AbilityType AbilityType => abilityType;
        
        public virtual void Initialize(GameObject owner)
        {
            Owner = owner;
            _timer = CurrentExecuteInterval;
        }
        
        public void UpdateTimer(float deltaTime)
        {
            _timer = Mathf.Min(_timer + Mathf.Max(0f, deltaTime), CurrentExecuteInterval);
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
