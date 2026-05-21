using System;
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
    }
}
