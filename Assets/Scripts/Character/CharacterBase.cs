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

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rb.gravityScale = 0f;
            
            Col = GetComponent<BoxCollider2D>();
            
            MoveSpeed = BaseMoveSpeed;
        }

        public virtual void DoAttack() { }
        
        public virtual void SetMoveSpeed(float value) { MoveSpeed = value; }
    }
}
