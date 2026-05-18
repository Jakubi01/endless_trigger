using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterBase : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private BoxCollider2D _col;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
        }
    }
}
