using UnityEngine;
using Items.ProjectileManager;

namespace Items.Weapon
{
    public class Boomerang : MonoBehaviour
    {
        public float speed = 12f;
        public float spinSpeed = 720f;
        private float _maxDistance = 10f;

        private Transform _owner;
        private Projectile _projectile;
        private Vector2 _dir;
        private Vector2 _startPos;
        private bool _returning;

        private void Awake()
        {
            _projectile = GetComponent<Projectile>();
        }
        
        public void Throw(Transform owner, Vector2 direction)
        {
            _owner = owner;
            _dir = direction.normalized;
            _startPos = transform.position;
            _returning = false;

            if (TryGetComponent(out Rigidbody2D rb)) rb.linearVelocity = Vector2.zero;

            _projectile.SetLifeTime(10f);
        }

        private void Update()
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            if (!_returning)
            {
                transform.position += (Vector3)(_dir * (speed * Time.deltaTime));
                if (Vector2.Distance(_startPos, transform.position) >= _maxDistance)
                {
                    _returning = true;
                    _projectile.ResetHits();
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _owner.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, _owner.position) < 0.1f)
                    _projectile.Release();
            }
        }
    }
}