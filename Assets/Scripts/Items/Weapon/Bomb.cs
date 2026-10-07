using System;
using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private GameObject bombAreaPrefab;
        
        private EnemyCharacterBase _target;
        private float _speed;
        private float _damagePerSecond;
        private float _areaDuration;
        private float _areaRadius;
        private Action _onAreaFinished;
        private bool _detonated;
        
        private void Awake()
        {
            var circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.18f;
        }
        
        public void Initialize(EnemyCharacterBase target, float speed, float damagePerSecond, float areaDuration,
            float areaRadius, Action onAreaFinished)
        {
            _target = target;
            _speed = Mathf.Max(0f, speed);
            _damagePerSecond = Mathf.Max(0f, damagePerSecond);
            _areaDuration = Mathf.Max(0f, areaDuration);
            _areaRadius = Mathf.Max(0.1f, areaRadius);
            _onAreaFinished = onAreaFinished;
        }

        private void Update()
        {
            if (_detonated) return;

            if (!_target || !_target.IsAlive)
            {
                Detonate();
                return;
            }

            Vector3 targetPosition = _target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            if ((transform.position - targetPosition).sqrMagnitude <= 0.04f)
                Detonate();
        }

        private void Detonate()
        {
            if (_detonated) return;
            _detonated = true;

            if (bombAreaPrefab)
            {
                Instantiate(bombAreaPrefab, transform);
                Destroy(gameObject);
                return;
            }

            GameObject areaObject = new GameObject("ClayBombArea");
            areaObject.transform.position = transform.position;
            SpriteRenderer spriteRenderer = areaObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = BombArea.VisualSprite;
            spriteRenderer.color = new Color(0.9f, 0.34f, 0.12f, 0.58f);
            spriteRenderer.sortingOrder = -1;
            areaObject.AddComponent<CircleCollider2D>().isTrigger = true;

            areaObject.AddComponent<BombArea>().Initialize(
                _damagePerSecond,
                _areaDuration,
                _areaRadius,
                _onAreaFinished);

            Destroy(gameObject);
        }
    }
}
