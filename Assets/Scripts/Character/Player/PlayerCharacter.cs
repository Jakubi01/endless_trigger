using System.Collections.Generic;
using Components;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Character.Player
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PlayerLevelComponent))]
    public class PlayerCharacter : CharacterBase, IDamageable
    {
        [Header("Status")]
        [SerializeField] private float baseMoveSpeed = 5f;

        private readonly List<WeaponAttackBase> _equippedWeapons = new();
        private EnemyManager _enemyManager;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;

        public float ExperiencePickupRange => _levelComponent ? _levelComponent.ExperiencePickupRange : 2.5f;

        protected override void Awake()
        {
            base.Awake();

            SetMoveSpeed(baseMoveSpeed);
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
                _levelComponent.LeveledUp -= ApplyLevelUpUpgrade;
            }
        }

        private void FixedUpdate()
        {
            ProcessTranslation();
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

            if (Mathf.Abs(MoveInput.x) < 0.01f) return;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (MoveInput.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }

        public override void DoAttack()
        {
        }

        public GameObject FindNearestFromCharacter(float range)
        {
            ResolveEnemyManager();
            return _enemyManager ? _enemyManager.FindNearestEnemy(transform, range) : null;
        }

        public GameObject FindFarthestFromCharacter(float range)
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
            _levelComponent.LeveledUp += ApplyLevelUpUpgrade;
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            
            StopAllWeapons();
            gameObject.SetActive(false);
            UIManager.Instance.ShowGameOverWindow();
        }

        private void ApplyLevelUpUpgrade()
        {
            if (_equippedWeapons.Count == 0) return;

            int randomIndex = Random.Range(0, _equippedWeapons.Count);
            _equippedWeapons[randomIndex].ApplyRandomUpgrade();
        }

        private void ResolveEnemyManager()
        {
            if (_enemyManager) return;

            _enemyManager = EnemyManager.Instance ? EnemyManager.Instance : FindFirstObjectByType<EnemyManager>();
        }
    }
}
