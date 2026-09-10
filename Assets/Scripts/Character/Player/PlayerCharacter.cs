using System.Collections.Generic;
using System.Globalization;
using Components;
using Effects.DamageText;
using Items.Weapon;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

namespace Character.Player
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PlayerLevelComponent))]
    public class PlayerCharacter : CharacterBase, IDamageable
    {
        [Header("Status")]
        [SerializeField] private float baseMoveSpeed = 5f;

        [Header("Level Up")]
        [SerializeField] private List<PlayerUpgradeDefinition> upgradeDefinitions = new();

        private readonly List<WeaponAttackBase> _equippedWeapons = new();
        private EnemyManager _enemyManager;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;
        private float _currentMoveSpeed;
        private readonly Dictionary<PlayerUpgradeDefinition, int> _upgradeStacks = new();

        public float ExperiencePickupRange => _levelComponent ? _levelComponent.ExperiencePickupRange : 2.5f;

        protected override void Awake()
        {
            base.Awake();

            _currentMoveSpeed = baseMoveSpeed;
            SetMoveSpeed(_currentMoveSpeed);
            InitializeStatusComponents();

            var swordAttack = GetComponentInChildren<SwordAttack>();
            var spearAttack = GetComponentInChildren<SpearAttack>();
            if (!swordAttack || !spearAttack)
            {
                Debug.LogError("PlayerPrefab 자식 오브젝트에 weapon이 없음.");
                return;
            }
            
            _equippedWeapons.Add(swordAttack);
            _equippedWeapons.Add(spearAttack);
            StartAllWeapons();

            _enemyManager = FindFirstObjectByType<EnemyManager>();
            if (!_enemyManager)
            {
                Debug.LogError("씬에 EnemyManager가 없음.");
                return;
            }

            _enemyManager.Player = this;

            var es = _enemyManager.GetComponent<EnemySpawner>();
            if (es)
            {
                es.Player = this;
            }
            
            GetComponent<SpriteRenderer>().sortingLayerName = "Player";
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

        public void StartAllWeapons()
        {
            foreach (var weapon in _equippedWeapons)
            {
                weapon.StartFiring();
            }
        }

        public void StopAllWeapons()
        {
            foreach (var weapon in _equippedWeapons)
            {
                weapon.StopFiring();
            }
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            MoveInput = moveInput;
        }

        public override void DoAttack()
        {
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
            
            StopAllWeapons();
            gameObject.SetActive(false);
            UIManager.Instance.ShowGameOverWindow();
        }

        public int GetUpgradeStackCount(PlayerUpgradeDefinition definition)
        {
            return definition && _upgradeStacks.TryGetValue(definition, out int stackCount) ? stackCount : 0;
        }

        public void ApplyUpgrade(PlayerUpgradeDefinition definition)
        {
            if (!definition || !definition.CanApply(this)) return;

            definition.Apply(this);
            _upgradeStacks[definition] = GetUpgradeStackCount(definition) + 1;
        }

        // public void AddMoveSpeed(float amount)
        // {
        //     if (amount <= 0f) return;
        //     _currentMoveSpeed += amount;
        //     SetMoveSpeed(_currentMoveSpeed);
        // }
        //
        // public void AddMaxHealth(float amount) => _healthComponent?.AddMaxHealth(amount);
        // public void Heal(float amount) => _healthComponent?.Heal(amount);
        //
        // public void UpgradeWeaponDamage(PlayerWeaponTarget target, float amount)
        // {
        //     foreach (WeaponAttackBase weapon in GetWeapons(target)) weapon.AddDamageBonus(amount);
        // }
        //
        // public void UpgradeWeaponAttackSpeed(PlayerWeaponTarget target, float amount)
        // {
        //     foreach (WeaponAttackBase weapon in GetWeapons(target)) weapon.AddAttackSpeedBonus(amount);
        // }
        //
        // private IEnumerable<WeaponAttackBase> GetWeapons(PlayerWeaponTarget target)
        // {
        //     foreach (WeaponAttackBase weapon in _equippedWeapons)
        //     {
        //         if (target == PlayerWeaponTarget.All ||
        //             (target == PlayerWeaponTarget.Sword && weapon is SwordAttack) ||
        //             (target == PlayerWeaponTarget.Spear && weapon is SpearAttack))
        //             yield return weapon;
        //     }
        // }

        private void ShowLevelUpSelection()
        {
            List<PlayerUpgradeDefinition> choices = PlayerUpgradeDefinition.GetRandomChoices(this, upgradeDefinitions, 3);
            if (choices.Count == 0)
            {
                Debug.LogError("PlayerCharacter에 선택 가능한 PlayerUpgradeDefinition이 없습니다.", this);
                _levelComponent.CompleteUpgradeSelection();
                return;
            }

            if (UIManager.Instance)
            {
                UIManager.Instance.ShowLevelUpSelection(choices, SelectUpgrade);
            }
            else
            {
                SelectUpgrade(choices[0]);
            }
        }

        private void SelectUpgrade(PlayerUpgradeDefinition definition)
        {
            ApplyUpgrade(definition);
            SpawnLevelUpVfx(_levelComponent ? _levelComponent.LevelUpVFXPrefab : null);
            _levelComponent.CompleteUpgradeSelection();
        }

        private void SpawnLevelUpVfx(GameObject levelUpVFXPrefab)
        {
            if (_equippedWeapons.Count == 0) return;

            // TODO : 이 랜덤 무기 강화를 덱으로 넣어버리고 여기에 덱 카드 선택 추가
            // enhance weapon status
            if (!levelUpVFXPrefab) return;
            
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
