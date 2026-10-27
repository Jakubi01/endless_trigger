using Components;
using UnityEngine;

namespace Items.Weapon
{
    public class BossProjectile : MonoBehaviour
    {
        private float _damage;
        private float _timer;
        private bool _pierce;
        private bool _hit;
        private bool _ended;
        private GameObject _hitFx;
        private GameObject _endFx;

        public void Init(float damage, bool pierce, float lifetime, GameObject hitFx, GameObject endFx)
        {
            _damage = damage;
            _pierce = pierce;
            _timer = lifetime;
            _hitFx = hitFx;
            _endFx = endFx;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f) End();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hit || !other.CompareTag("Player")) return;

            var health = other.GetComponentInParent<HealthComponent>();
            if (!health) return;

            _hit = true;
            health.TakeDamage(_damage);
            Spawn(_hitFx);
            if (!_pierce) End();
        }

        private void End()
        {
            if (_ended) return;
            _ended = true;
            Spawn(_endFx);
            Destroy(gameObject);
        }

        private void Spawn(GameObject fx)
        {
            if (fx) Destroy(Instantiate(fx, transform.position, transform.rotation), 1f);
        }
    }
}