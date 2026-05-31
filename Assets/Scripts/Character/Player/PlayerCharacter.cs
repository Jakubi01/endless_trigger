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

        [Header("Weapon")]
        [SerializeField] private GameObject shotGunPrefab;
        [SerializeField] private GameObject sniperPrefab;
        [SerializeField] private Transform shotgunMountTransform;
        [SerializeField] private Transform sniperMountTransform;

        private readonly List<WeaponBase> _equippedWeapons = new();
        private EnemyManager _enemyManager;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;

        public float ExperiencePickupRange => _levelComponent ? _levelComponent.ExperiencePickupRange : 2.5f;

        protected override void Awake()
        {
            base.Awake();

            SetMoveSpeed(baseMoveSpeed);
            InitializeStatusComponents();
            InitializeWeapons();

            _enemyManager = FindFirstObjectByType<EnemyManager>();
            if (!_enemyManager)
            {
                Debug.LogError("씬에 EnemyManager가 없음.");
            }
            
            GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        }

        private void OnDestroy()
        {
            if (_healthComponent)
            {
                _healthComponent.Died -= HandleDeath;
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

        private void InitializeWeapons()
        {
            if (shotGunPrefab != null)
            {
                Transform parent = shotgunMountTransform != null ? shotgunMountTransform : transform;
                GameObject sgObj = Instantiate(shotGunPrefab, parent);
                sgObj.transform.localPosition = Vector3.zero;
                sgObj.transform.localRotation = Quaternion.identity;

                if (sgObj.TryGetComponent(out WeaponBase shotgun))
                {
                    _equippedWeapons.Add(shotgun);
                    shotgun.owner = this;
                }
            }

            if (sniperPrefab != null)
            {
                Transform parent = sniperMountTransform != null ? sniperMountTransform : transform;
                GameObject snObj = Instantiate(sniperPrefab, parent);
                snObj.transform.localPosition = Vector3.zero;
                snObj.transform.localRotation = Quaternion.identity;

                if (snObj.TryGetComponent(out WeaponBase sniper))
                {
                    _equippedWeapons.Add(sniper);
                    sniper.owner = this;
                }
            }

            StartAllWeapons();
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

            _healthComponent.Died += HandleDeath;
            _levelComponent.LeveledUp += ApplyLevelUpUpgrade;
        }

        private void HandleDeath()
        {
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
