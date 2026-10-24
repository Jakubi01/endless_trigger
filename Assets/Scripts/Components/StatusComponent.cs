using Character.Player;
using Components.AbilitySystemComponent.Abilities;
using Components.AbilitySystemComponent;
using Items.Weapon;
using UnityEngine;
using UserInterface;

namespace Components
{
    public class StatusComponent : ComponentBase
    {
        [SerializeField] private CoolTimePanel coolTimePanel;

        private PlayerCharacter _playerCharacter;
        private SwordAttack _swordAttack;
        private SpearAttack _spearAttack;
        private AbilitySystem _abilitySystem;
        private StatusWindow _statusWindow;

        protected override void Awake()
        {
            base.Awake();

            _playerCharacter = Owner.GetComponent<PlayerCharacter>();
            _swordAttack = _playerCharacter.GetComponentInChildren<SwordAttack>();
            _spearAttack = _playerCharacter.GetComponentInChildren<SpearAttack>();
            _abilitySystem = Owner.GetComponent<AbilitySystem>();
        }

        private void Start()
        {
            _statusWindow = FindFirstObjectByType<StatusWindow>();
            if (!_statusWindow) return;

            if (_swordAttack)
            {
                _swordAttack.SetCoolTimePanel(SpawnCoolTimePanel());
            }

            if (_spearAttack)
            {
                _spearAttack.SetCoolTimePanel(SpawnCoolTimePanel());
            }

            if (!_abilitySystem) return;

            _abilitySystem.OnAbilityAdded += HandleAbilityAdded;
            foreach (AbilityBase ability in _abilitySystem.Abilities)
            {
                HandleAbilityAdded(ability);
            }
        }

        private void OnDestroy()
        {
            if (_abilitySystem)
            {
                _abilitySystem.OnAbilityAdded -= HandleAbilityAdded;
            }
        }

        private void HandleAbilityAdded(AbilityBase ability)
        {
            if (!ability) return;

            ability.SetCoolTimePanel(SpawnCoolTimePanel());
        }

        public CoolTimePanel SpawnCoolTimePanel()
        {
            return !coolTimePanel || !_statusWindow
                ? null
                : Instantiate(coolTimePanel, _statusWindow.transform);
        }
    }
}
