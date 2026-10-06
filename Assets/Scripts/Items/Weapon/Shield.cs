using Items.ProjectileManager;
using UnityEngine;

namespace Items.Weapon
{
    public class Shield : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float hitInterval = 0.5f;

        private Transform _owner;
        private Projectile _projectile;
        private float _angle;
        private float _hitTimer;

        private void Awake()
        {
            _projectile = GetComponent<Projectile>();
        }

        public void Init(Transform owner, float startAngle)
        {
            _owner = owner;
            _angle = startAngle;
            _hitTimer = hitInterval;
            UpdatePosition();
        }

        private void Update()
        {
            if (!_owner) return;

            _angle += speed / radius * Mathf.Rad2Deg * Time.deltaTime;
            UpdatePosition();

            _hitTimer -= Time.deltaTime;
            if (_hitTimer <= 0f)
            {
                _hitTimer = hitInterval;
                _projectile.ResetHits();
            }
        }

        private void UpdatePosition()
        {
            float rad = _angle * Mathf.Deg2Rad;
            transform.position = _owner.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }
    }
}