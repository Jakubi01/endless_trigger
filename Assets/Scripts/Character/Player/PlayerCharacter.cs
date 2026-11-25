#define DEBUG_ABILITY
#undef DEBUG_ABILITY

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Character.Player.Upgrade;
using Components;
using Components.AbilitySystemComponent;
using Components.AbilitySystemComponent.Abilities;
using Effects.DamageText;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Character.Player
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PlayerLevelComponent))]
    [RequireComponent(typeof(AbilitySystem))]
    public class PlayerCharacter : CharacterBase, IDamageable
    {
        [Header("Status")]
        [SerializeField] private float baseMoveSpeed = 5f;

        [Header("Level Up")]
        [SerializeField] private List<PlayerUpgradeDefinition> upgradeDefinitions = new();

        public readonly List<WeaponAttackBase> EquippedWeapons = new();
        private EnemyManager _enemyManager;
        private UIManager _uiManagerInstance;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;
        private float _currentMoveSpeed;
        private readonly Dictionary<PlayerUpgradeDefinition, int> _upgradeStacks = new();
        private const string StateName = "Death";

        public float ExperiencePickupRange => _levelComponent ? _levelComponent.ExperiencePickupRange : 2.5f;
        
        [SerializeField] private AbilityBase testAbility;

        protected override void Awake()
        {
            base.Awake();

            _currentMoveSpeed = baseMoveSpeed;
            SetMoveSpeed(_currentMoveSpeed);
            InitializeStatusComponents();

            var swordAttack = GetComponentInChildren<SwordAttack>();
            var spearAttack = GetComponentInChildren<SpearAttack>();
            if (!swordAttack || !spearAttack) return;
            
            EquippedWeapons.Add(swordAttack);
            EquippedWeapons.Add(spearAttack);
            StartAllWeapons();

            _enemyManager = FindFirstObjectByType<EnemyManager>();
            if (!_enemyManager) return;

            _enemyManager.Player = this;

            var es = _enemyManager.GetComponent<EnemySpawner>();
            if (es)
            {
                es.Player = this;
            }
            
            GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        }

        private void Start()
        {
            _uiManagerInstance = UIManager.Instance;
#if DEBUG_ABILITY
            if (!testAbility) return;
            var sys = GetComponent<AbilitySystem>();
            if (sys) sys.AddAbility(testAbility);
#endif
        }

        private void OnDestroy()
        {
            if (_healthComponent)
            {
                _healthComponent.Died -= OnDeath;
            }

            if (_levelComponent)
            {
                _levelComponent.LeveledUp -= ShowLevelUpSelection;
            }
        }

        private void FixedUpdate()
        {
            ProcessTranslation();
            UpdateAnimation();
        }

        protected override bool UpdateAnimation()
        {
            var result = base.UpdateAnimation();
            animator.SetBool(AnimatorParamToHash.IsRunning, result);

            return result;
        }

        private void StartAllWeapons()
        {
            foreach (var weapon in EquippedWeapons)
            {
                weapon.StartFiring();
            }
        }

        private void StopAllWeapons()
        {
            foreach (var weapon in EquippedWeapons)
            {
                weapon.StopFiring();
            }
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            MoveInput = moveInput;
        }

        public override GameObject FindNearestFromCharacter(float range)
        {
            ResolveEnemyManager();
            return _enemyManager ? _enemyManager.FindNearestEnemy(transform, range) : null;
        }

        public override GameObject FindFarthestFromCharacter(float range)
        {
            ResolveEnemyManager();
            return _enemyManager ? _enemyManager.FindFarthestEnemy(transform, range) : null;
        }

        public void GainExperience(int amount)
        {
            _levelComponent?.GainExperience(amount);
        }

        public void TakeDamage(float amount)
        {
            _healthComponent?.TakeDamage(amount);
            DamageTextManager.Instance?.ShowDamageText(transform.position, amount.ToString(CultureInfo.InvariantCulture), true);
        }

        private void ProcessTranslation()
        {
            Move(MoveInput);
        }

        private void InitializeStatusComponents()
        {
            _healthComponent = GetComponent<HealthComponent>();
            if (!_healthComponent)
            {
                _healthComponent = gameObject.AddComponent<HealthComponent>();
            }

            _levelComponent = GetComponent<PlayerLevelComponent>();
            if (!_levelComponent)
            {
                _levelComponent = gameObject.AddComponent<PlayerLevelComponent>();
            }

            _healthComponent.Died += OnDeath;
            _levelComponent.LeveledUp += ShowLevelUpSelection;
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            
            GameManager.Instance?.EndRun();
            
            StopAllWeapons();
            animator.SetTrigger(AnimatorParamToHash.PlayerDeath);
            
            var length = AnimationExtension.GetAnimClipLength(animator, StateName);
            StartCoroutine(WaitForDiedAnimationEnd(length));
        }

        private IEnumerator WaitForDiedAnimationEnd(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            
            _uiManagerInstance.ShowGameOverWindow();
        }

        public int GetUpgradeStackCount(PlayerUpgradeDefinition definition)
        {
            return definition && _upgradeStacks.TryGetValue(definition, out int stackCount) ? stackCount : 0;
        }

        private void ApplyUpgrade(PlayerUpgradeDefinition definition)
        {
            if (!definition || !definition.CanApply(this)) return;

            definition.Apply(this);
            _upgradeStacks[definition] = GetUpgradeStackCount(definition) + 1;
        }

        public void AddMoveSpeed(float amount)
        {
            if (amount <= 0f) return;
            _currentMoveSpeed += amount;
            SetMoveSpeed(_currentMoveSpeed);
        }

        private void ShowLevelUpSelection()
        {
            if (!_uiManagerInstance) return;
            
            var choices = PlayerUpgradeDefinition.GetRandomChoices(this, upgradeDefinitions, 3);
            if (choices.Count == 0)
            {
                Debug.LogError("PlayerCharacter에 선택 가능한 PlayerUpgradeDefinition이 없습니다.", this);
                _levelComponent.CompleteUpgradeSelection();
                return;
            }

            _uiManagerInstance.SetGameHUDVisibility(false);
            _uiManagerInstance.ShowLevelUpSelection(choices, SelectUpgrade);
        }

        private void SelectUpgrade(PlayerUpgradeDefinition definition)
        {
            if (!_levelComponent) return;
            
            ApplyUpgrade(definition);
            SpawnLevelUpVfx(_levelComponent ? _levelComponent.LevelUpVFXPrefab : null);
            _levelComponent.CompleteUpgradeSelection();
            _uiManagerInstance.SetGameHUDVisibility(true);
        }

        private void SpawnLevelUpVfx(GameObject levelUpVFXPrefab)
        {
            if (EquippedWeapons.Count == 0 || !levelUpVFXPrefab) return;
            
            var levelUpVFX = Instantiate(levelUpVFXPrefab, transform);
            if (!levelUpVFX) return;

            levelUpVFX.transform.localPosition = Vector3.left * 2f;
        }

        private void ResolveEnemyManager()
        {
            if (_enemyManager) return;

            _enemyManager = EnemyManager.Instance ? EnemyManager.Instance : FindFirstObjectByType<EnemyManager>();
        }
    }
}
