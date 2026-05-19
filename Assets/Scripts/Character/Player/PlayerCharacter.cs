using System;
using UnityEngine;

namespace Character.Player
{
    public class PlayerCharacter : CharacterBase
    {
        [Header("Weapon")] 
        [SerializeField] private GameObject shotGunPrefab;
        [SerializeField] private GameObject sniperPrefab;
        [NonSerialized] public float ShotGunFireInterval;
        [NonSerialized] public float SniperFireInterval;
        private float _timer;

        protected override void Awake()
        {
            base.Awake();

            ShotGunFireInterval = 3f;
            SniperFireInterval = 5f;
        }
        
        private void FixedUpdate()
        {
            ProcessTranslation();
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