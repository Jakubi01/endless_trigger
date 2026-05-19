using System;
using System.Collections.Generic;
using Items.Weapon;
using UnityEngine;

namespace Character.Player
{
    public class PlayerCharacter : CharacterBase
    {
        [Header("Weapon")] 
        [SerializeField] private GameObject shotGunPrefab;
        [SerializeField] private GameObject sniperPrefab;
        [SerializeField] private Transform shotgunMountTransform;
        [SerializeField] private Transform sniperMountTransform;
        private List<WeaponBase> _equippedWeapons = new();
        
        protected override void Awake()
        {
            base.Awake();

            InitializeWeapons();
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
                    _equippedWeapons.Add(shotgun);
            }

            if (sniperPrefab != null)
            {
                Transform parent = sniperMountTransform != null ? sniperMountTransform : transform;
                GameObject snObj = Instantiate(sniperPrefab, parent);
                snObj.transform.localPosition = Vector3.zero;
                snObj.transform.localRotation = Quaternion.identity;
                
                if (snObj.TryGetComponent(out WeaponBase sniper))
                    _equippedWeapons.Add(sniper);
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
        
        private void ProcessTranslation()
        {
            transform.position += (Vector3)MoveInput * (MoveSpeed * Time.fixedDeltaTime);
        }

        public override void DoAttack()
        {
            
        }
    }
}