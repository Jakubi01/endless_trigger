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
        public float moveSpeed;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Col = GetComponent<BoxCollider2D>();
            
            moveSpeed = BaseMoveSpeed;
        }
    }
}
