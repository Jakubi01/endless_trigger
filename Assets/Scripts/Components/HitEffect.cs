using System.Collections;
using UnityEngine;

namespace Components
{
    public class HitEffect : ComponentBase
    {
        [SerializeField] private Color hitColor = Color.red;
        private const float Duration = 0.1f;

        private HealthComponent _health;
        private SpriteRenderer _sprite;
        private Color _baseColor;
        private WaitForSeconds _wait;
        private Coroutine _routine;

        protected override void Awake()
        {
            base.Awake();
            
            _health = GetComponent<HealthComponent>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _baseColor = _sprite.color;
            _wait = new WaitForSeconds(Duration);
            _health.Died += () => _sprite.color = _baseColor;
        }

        private void OnEnable()
        {
            _sprite.color = _baseColor;
            _health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
            _routine = null;
        }

        private void OnDamaged(float _)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            _sprite.color = hitColor;
            yield return _wait;
            _sprite.color = _baseColor;
            _routine = null;
        }
    }
}