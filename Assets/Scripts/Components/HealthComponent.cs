using System;
using UnityEngine;

namespace Components
{
    public class HealthComponent : ComponentBase
    {
        [Header("Value")]
        [SerializeField] private float maxHealth = 100f;
        private float _currentHealth;

        public event Action Died;
        public event Action HealthChanged;
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
            else
            {
                HealthChanged?.Invoke();
            }
        }

        public void ResetHealth()
        {
            _currentHealth = maxHealth;
            HealthChanged?.Invoke();
        }

        public void AddMaxHealth(float amount, bool healByAddedAmount = true)
        {
            if (amount <= 0f) return;

            maxHealth += amount;
            _currentHealth = Mathf.Min(maxHealth, _currentHealth + (healByAddedAmount ? amount : 0f));
            HealthChanged?.Invoke();
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || _currentHealth <= 0f) return;

            _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
            HealthChanged?.Invoke();
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || _currentHealth <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            HealthChanged?.Invoke();

            if (_currentHealth <= 0f)
            {
                Died?.Invoke();
            }
        }
    }
}
