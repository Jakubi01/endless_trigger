using System;
using Managers;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterBase : MonoBehaviour
    {
        protected Rigidbody2D Rb;
        protected BoxCollider2D Col;

        private const float BaseMoveSpeed = 5f;
        protected Vector2 MoveInput;
        [NonSerialized] protected float MoveSpeed;
        public Rigidbody2D CharacterRigidbody => Rb;
        public Collider2D CharacterCollider => Col;
        
        [Header("Events")]
        [SerializeField] private AudioClip deathSound;
        private static float _lastDeathSoundTime;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            Rb.gravityScale = 0f;
            
            Col = GetComponent<BoxCollider2D>();
            
            MoveSpeed = BaseMoveSpeed;
        }

        public virtual void DoAttack() { }
        
        public virtual void SetMoveSpeed(float value) { MoveSpeed = value; }

        public virtual void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            Rb.MovePosition(Rb.position + direction * (MoveSpeed * Time.fixedDeltaTime));
        }

        public virtual void MoveToward(Vector2 worldPosition)
        {
            Vector2 direction = (worldPosition - Rb.position).normalized;
            Move(direction);
        }

        protected virtual void OnDeath()
        {
            PlayDeathSound();
        }
        
        protected void PlayDeathSound()
        {
            if (Time.time - _lastDeathSoundTime < 0.05f) 
                return; 

            _lastDeathSoundTime = Time.time;
            SoundManager.Instance.PlaySFX(deathSound);
        }
    }
}
