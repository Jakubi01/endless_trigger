using System;
using UnityEngine;

namespace Items.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifeTime = 3f;

        private Rigidbody2D _rb;
        private Action<GameObject> _returnToPool;
        private float _lifeTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
        }

        public void Initialize(Vector2 direction, Action<GameObject> returnAction)
        {
            _returnToPool = returnAction;
            _lifeTimer = lifeTime;
            
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            
            _rb.position = transform.position;
            _rb.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
            {
                ReleaseToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                ReleaseToPool();
            }
        }

        private void ReleaseToPool()
        {
            _returnToPool?.Invoke(gameObject);
        }
    }
}