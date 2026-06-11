using System;
using System.Collections.Generic;
using Character;
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
        private Collider2D _col;
        private Animator _animator;
        private Action<GameObject> _returnToPool;
        private float _lifeTimer;
        private float _damage;
        private int _remainingHits;
        private bool _isReleased;
        private bool _snapToParent;
        private Transform _parentTarget;
        private readonly List<GameObject> _overlappedObjects = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;

            _col = GetComponent<CircleCollider2D>();
            _col.isTrigger = true;
            
            _animator = GetComponent<Animator>();

            _overlappedObjects.Clear();
        }

        public void Initialize(Vector2 direction, Action<GameObject> returnAction, CharacterBase owner, float damage = 0f,
            int pierceCount = 0, bool bSnapToParent = false, Transform snapPosition = null)
        {
            _returnToPool = returnAction;
            _lifeTimer = lifeTime;
            _damage = damage;
            _remainingHits = Mathf.Max(1, pierceCount + 1);
            _isReleased = false;
            
            if (bSnapToParent)
            {
                _snapToParent = true;
                _parentTarget = snapPosition;
            }

            _rb.angularVelocity = 0f;
            _rb.position = transform.position;
            _rb.linearVelocity = Vector2.zero;
            _rb.linearVelocity = direction.normalized * speed;
            if (_snapToParent && _parentTarget != null)
            {
                _rb.linearVelocity = Vector2.zero;
            }
            else
            {
                _rb.linearVelocity = direction.normalized * speed;
            }

            if (!_animator) return;
            
            // _animator.Play()
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

        private void FixedUpdate()
        {
            if (!_snapToParent || !_parentTarget) return;

            _rb.MovePosition(_parentTarget.position);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isReleased) return;
            if (!collision.TryGetComponent(out EnemyCharacterBase damageable)) return;

            if (_overlappedObjects.Contains(collision.gameObject)) return;
            _overlappedObjects.Add(collision.gameObject);
            
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
            _overlappedObjects.Clear();
            _returnToPool?.Invoke(gameObject);
        }
    }
}
