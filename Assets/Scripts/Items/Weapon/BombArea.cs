using System;
using System.Collections.Generic;
using Character;
using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BombArea : MonoBehaviour
    {
        private Animator _animator;
        private const float DamageTickInterval = 1f;
        private float _damagePerSecond;
        private float _duration;
        private float _radius;
        private float _elapsed;
        private float _nextDamageTime;
        private Action _onFinished;
        private readonly HashSet<EnemyCharacterBase> _damagedEnemies = new();

        private void Awake()
        {
            var circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.5f;
            
            _animator = GetComponent<Animator>();
        }
        
        public void Initialize(float damagePerSecond, float duration, float radius, Action onFinished)
        {
            _damagePerSecond = Mathf.Max(0f, damagePerSecond);
            _duration = Mathf.Max(0f, duration);
            _radius = Mathf.Max(0.1f, radius);
            _onFinished = onFinished;
            _elapsed = 0f;
            _nextDamageTime = 0f;

            transform.localScale = Vector3.one * (_radius * 2f);
            
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.Execute);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _duration)
            {
                Finish();
                return;
            }

            if (_elapsed >= _nextDamageTime && _nextDamageTime < _duration)
            {
                ApplyDamage();
                _nextDamageTime += DamageTickInterval;
            }
        }

        private void ApplyDamage()
        {
            _damagedEnemies.Clear();
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, _radius);

            foreach (Collider2D overlap in overlaps)
            {
                if (!overlap || !overlap.TryGetComponent(out EnemyCharacterBase enemy) || !enemy.IsAlive)
                    continue;

                if (_damagedEnemies.Add(enemy))
                    enemy.TakeDamage(_damagePerSecond);
            }
        }

        private void Finish()
        {
            Action callback = _onFinished;
            _onFinished = null;
            callback?.Invoke();

            if (_animator)
                _animator.enabled = false;
                
            Destroy(gameObject);
        }
    }
}