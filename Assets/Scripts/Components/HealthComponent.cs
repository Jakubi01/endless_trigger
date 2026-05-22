using System;
using UnityEngine;

namespace Components
{
    public class HealthComponent : ComponentBase
    {
        [SerializeField] private float maxHealth = 100f;

        private float _currentHealth;

        public event Action Died;
        public bool IsAlive => _currentHealth > 0f;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        protected override void Awake()
        {
            base.Awake();
            ResetHealth();
        }

        public void Initialize(float newMaxHealth, bool fillHealth = true)
        {
            maxHealth = Mathf.Max(1f, newMaxHealth);
            if (fillHealth)
            {
                ResetHealth();
            }
        }

        public void ResetHealth()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || _currentHealth <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            if (_currentHealth <= 0f)
            {
                Died?.Invoke();
            }
        }
    }
}
