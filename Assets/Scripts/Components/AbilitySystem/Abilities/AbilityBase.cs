using UnityEngine;

namespace Components.AbilitySystem
{
    public enum AbilityType
    {
        Test
    }
}

namespace Components.AbilitySystem.Abilities
{
    public abstract class AbilityBase : ScriptableObject
    {
        protected GameObject Owner;
        private float _cooldown;
        private float _baseExecuteInterval;
        private float _executeSpeedMultiplier = 1f;
        private float _timer;
        
        public float CurrentExecuteInterval => Mathf.Max(0.1f, (_baseExecuteInterval + _cooldown) / _executeSpeedMultiplier);
        public AbilityType AbilityType { get; protected set; }
        
        public virtual void Initialize(GameObject owner)
        {
            Owner = owner;
            _timer = CurrentExecuteInterval;
        }
        
        public void UpdateTimer(float deltaTime)
        {
            if (_timer < CurrentExecuteInterval)
            {
                _timer += deltaTime;
            }
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
        
        public void AddExecuteSpeedBonus(float multiplierBonus)
        {
            _executeSpeedMultiplier = Mathf.Max(0.01f, _executeSpeedMultiplier + multiplierBonus);
        }

        public virtual void Upgrade() { /* 여기에 업그레이드 로직 */}
    }
}