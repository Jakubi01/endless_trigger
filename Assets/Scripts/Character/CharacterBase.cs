using System;
using Managers;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class CharacterBase : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private AudioClip deathSound;
        private static float _lastDeathSoundTime;
        private const float BaseMoveSpeed = 5f;
        
        protected Rigidbody2D Rb;
        protected BoxCollider2D Col;

        [NonSerialized] protected float MoveSpeed;
        protected Vector2 MoveInput;
        protected Animator animator;
        protected SpriteRenderer characterSprite;

        [NonSerialized] public float moveSpeedMultiplier = 1f;
        public Rigidbody2D CharacterRigidbody => Rb;
        public Collider2D CharacterCollider => Col;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            Rb.gravityScale = 0f;
            
            Col = GetComponent<BoxCollider2D>();
            Col.isTrigger = true;
            
            MoveSpeed = BaseMoveSpeed;
            
            characterSprite = GetComponent<SpriteRenderer>();
            
            animator = GetComponent<Animator>();
        }
        
        public virtual void SetMoveSpeed(float value) { MoveSpeed = value; }

        public virtual void Move(Vector2 direction)
        {
            MoveInput = direction;
            
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }
            
            if (characterSprite && Mathf.Abs(direction.x) > 0.01f)
            {
                characterSprite.flipX = direction.x < 0;
            }

            Rb.MovePosition(Rb.position + direction * (MoveSpeed * moveSpeedMultiplier * Time.fixedDeltaTime));
        }

        public virtual void MoveToward(Vector2 worldPosition)
        {
            Vector2 direction = (worldPosition - Rb.position).normalized;
            Move(direction);
        }

        protected virtual void OnDeath()
        {
            Rb.linearVelocity = Vector2.zero;
            // _animator.SetBool(AnimatorParamToHash.Death, true);
            
            PlayDeathSound();
        }
        
        protected void PlayDeathSound()
        {
            if (Time.time - _lastDeathSoundTime < 0.05f || !deathSound) 
                return; 

            _lastDeathSoundTime = Time.time;
            SoundManager.Instance.PlaySFX(deathSound);
        }
        
        protected virtual bool UpdateAnimation()
        {
            if (!animator) return false;
            
            return MoveInput.sqrMagnitude > 0.001f;
        }

        public virtual GameObject FindFarthestFromCharacter(float range) { return null;}
        public virtual GameObject FindNearestFromCharacter(float range) { return null; }
    }
}
