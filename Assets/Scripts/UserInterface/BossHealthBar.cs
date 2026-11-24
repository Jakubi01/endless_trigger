using Components;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider hpBarSlider;

        private HealthComponent _healthComponent;
        
        private void Awake()
        {
            hpBarSlider.interactable = false;
            hpBarSlider.value = 0;
        }
        
        public void Init(HealthComponent healthComponent)
        {
            if(!healthComponent) return;
            _healthComponent = healthComponent;
            BindHealthComponent();
        }
        
        private void OnDestroy()
        {
            UnBindHealthComponent();
        }

        private void BindHealthComponent()
        {
            if (!_healthComponent) return;
            _healthComponent.HealthChanged += RefreshHealth;
        }

        private void UnBindHealthComponent()
        {
            if (!_healthComponent) return;
            _healthComponent.HealthChanged -= RefreshHealth;
        }

        private void RefreshHealth()
        {
            if (!hpBarSlider || !_healthComponent) return;

            var maxHealth = _healthComponent.MaxHealth;
            var currentHealth = _healthComponent.CurrentHealth;
            
            hpBarSlider.minValue = 0f;
            hpBarSlider.maxValue = maxHealth;
            hpBarSlider.value = currentHealth;
        }
    }
}