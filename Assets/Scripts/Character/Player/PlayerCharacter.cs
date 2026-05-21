using System.Collections.Generic;
using Items.Weapon;
using Managers;
using UnityEngine;

namespace Character.Player
{
    public class PlayerCharacter : CharacterBase, IDamageable
    {
        [Header("Status")]
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private float baseMoveSpeed = 5f;
        [SerializeField] private int requiredExperiencePerLevel = 100;
        [SerializeField] private float experiencePickupRange = 2.5f;

        [Header("Weapon")]
        [SerializeField] private GameObject shotGunPrefab;
        [SerializeField] private GameObject sniperPrefab;
        [SerializeField] private Transform shotgunMountTransform;
        [SerializeField] private Transform sniperMountTransform;

        private readonly List<WeaponBase> _equippedWeapons = new();
        private EnemyManager _enemyManager;
        private float _currentHp;
        private int _currentExperience;
        private int _level = 1;

        public float ExperiencePickupRange => experiencePickupRange;

        protected override void Awake()
        {
            base.Awake();

            SetMoveSpeed(baseMoveSpeed);
            _currentHp = maxHp;
            InitializeWeapons();

            _enemyManager = FindFirstObjectByType<EnemyManager>();
            if (!_enemyManager)
            {
                Debug.LogError("씬에 EnemyManager가 없음.");
            }
            
            GetComponent<SpriteRenderer>().sortingLayerName = "Player";
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
            if (amount <= 0) return;

            _currentExperience += amount;
            while (_currentExperience >= requiredExperiencePerLevel)
            {
                _currentExperience -= requiredExperiencePerLevel;
                LevelUp();
            }
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || _currentHp <= 0f) return;

            _currentHp = Mathf.Max(0f, _currentHp - amount);
            if (_currentHp <= 0f)
            {
                StopAllWeapons();
                gameObject.SetActive(false);
            }
        }

        private void ProcessTranslation()
        {
            Move(MoveInput);
        }

        private void LevelUp()
        {
            _level++;
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
