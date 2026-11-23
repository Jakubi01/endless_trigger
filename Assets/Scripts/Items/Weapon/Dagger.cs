using Character.Enemy;
using Managers;
using UnityEngine;

namespace Items.Weapon
{
    public class Dagger : MonoBehaviour
    {
        [SerializeField] private GameObject destroyFx;
        [SerializeField] private AudioClip throwClip;

        private bool _hasHit;

        private void OnEnable()
        {
            _hasHit = false;
        }

        public void Init(Vector2 direction)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon) return;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            _hasHit = false;
            SoundManager.Instance.PlaySfx(throwClip);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasHit || !collision.TryGetComponent(out EnemyCharacterBase _)) return;

            _hasHit = true;
            if (!destroyFx) return;

            var fx = Instantiate(destroyFx, transform.position, Quaternion.identity);
            Destroy(fx, 1f);
        }
    }
}
