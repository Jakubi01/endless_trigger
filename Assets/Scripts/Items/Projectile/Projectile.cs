using System;
using Character.Enemy;
using UnityEngine;

namespace Items.Projectile
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifeTime = 3f;

        private Rigidbody2D _rb;
        private Action<GameObject> _returnToPool;
        private float _lifeTimer;
        private float _damage;
        private int _remainingHits;
        private bool _isReleased;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;

            CircleCollider2D projectileCollider = GetComponent<CircleCollider2D>();
            if (!projectileCollider)
            {
                projectileCollider = gameObject.AddComponent<CircleCollider2D>();
            }

            projectileCollider.isTrigger = true;
        }

        public void Initialize(Vector2 direction, Action<GameObject> returnAction, float damage = 0f, int pierceCount = 0)
        {
            _returnToPool = returnAction;
            _lifeTimer = lifeTime;
            _damage = damage;
            _remainingHits = Mathf.Max(1, pierceCount + 1);
            _isReleased = false;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.position = transform.position;
            _rb.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            if (_isReleased) return;

            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
            {
                ReleaseToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isReleased) return;
            if (!collision.TryGetComponent(out EnemyCharacterBase damageable)) return;

            damageable.TakeDamage(_damage);
            _remainingHits--;

            if (_remainingHits <= 0)
            {
                ReleaseToPool();
            }
        }

        private void ReleaseToPool()
        {
            if (_isReleased) return;

            _isReleased = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _returnToPool?.Invoke(gameObject);
        }
    }
}
