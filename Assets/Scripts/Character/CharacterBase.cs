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

        private BoxCollider2D _col;
        private SpriteRenderer _characterSprite;
        private float _moveSpeed;

        protected Vector2 MoveInput;

        public float MoveSpeedMultiplier { get; set; }
        public Animator animator;
        public Rigidbody2D CharacterRigidbody { get; private set; }

        protected virtual void Awake()
        {
            CharacterRigidbody = GetComponent<Rigidbody2D>();
            CharacterRigidbody.bodyType = RigidbodyType2D.Dynamic;
            CharacterRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            CharacterRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            CharacterRigidbody.gravityScale = 0f;
            
            _col = GetComponent<BoxCollider2D>();
            _col.isTrigger = true;
            
            _moveSpeed = BaseMoveSpeed;
            MoveSpeedMultiplier = 1f;
            
            _characterSprite = GetComponent<SpriteRenderer>();
            
            animator = GetComponent<Animator>();
        }

        protected virtual void SetMoveSpeed(float value) { _moveSpeed = value; }

        protected virtual void Move(Vector2 direction)
        {
            MoveInput = direction;
            
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }
            
            if (_characterSprite && Mathf.Abs(direction.x) > 0.01f)
            {
                _characterSprite.flipX = direction.x < 0;
            }

            CharacterRigidbody.MovePosition(CharacterRigidbody.position + direction * (_moveSpeed * MoveSpeedMultiplier * Time.fixedDeltaTime));
        }

        protected void MoveToward(Vector2 worldPosition)
        {
            Vector2 direction = (worldPosition - CharacterRigidbody.position).normalized;
            Move(direction);
        }

        protected virtual void OnDeath()
        {
            CharacterRigidbody.linearVelocity = Vector2.zero;
            // _animator.SetBool(AnimatorParamToHash.Death, true);
            
            PlayDeathSound();
        }

        private void PlayDeathSound()
        {
            if (Time.time - _lastDeathSoundTime < 0.05f || !deathSound) 
                return; 

            _lastDeathSoundTime = Time.time;
            SoundManager.Instance.PlaySfx(deathSound);
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
