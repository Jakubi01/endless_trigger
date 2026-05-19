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
        private Action<Projectile> _returnToPool;
        private float _lifeTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(Vector2 direction, Action<Projectile> returnAction)
        {
            _returnToPool = returnAction;
            _lifeTimer = lifeTime;
            
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
            _returnToPool?.Invoke(this);
        }
    }
}