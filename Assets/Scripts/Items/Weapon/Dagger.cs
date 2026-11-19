using Character;
using UnityEngine;

namespace Items.Weapon
{
    public class Dagger : MonoBehaviour
    {
        private Rigidbody2D _rb;
        
        private float _speed;
        private float _damage;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
        public void Init(Vector2 targetDir)
        {
            _speed = 2f;
            if(_rb is not null) _rb.linearVelocity = targetDir * _speed;
        }

        public void SetDamage(float damage) => _damage = damage;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") || !collision.CompareTag("Enemy")) return;
            if (!collision.TryGetComponent(out IDamageable damageable)) return;
            
            damageable.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}