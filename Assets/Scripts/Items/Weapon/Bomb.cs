using System;
using System.Collections;
using Character;
using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private GameObject bombAreaPrefab;
        
        private EnemyCharacterBase _target;
        private Animator _animator;
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
            circleCollider.radius = 0.5f;
            
            _animator = GetComponent<Animator>();
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
                if (!bombAreaPrefab) return;
                StartCoroutine(Detonate());
                return;
            }

            Vector3 targetPosition = _target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            
            if ((transform.position - targetPosition).sqrMagnitude <= 0.04f)
            {
                StartCoroutine(Detonate());
            }
        }

        private IEnumerator Detonate()
        {
            _detonated = true;

            if (_animator)
            {
                _animator.SetTrigger(AnimatorParamToHash.Execute);
                const string clipName = "BombAnim";
                var clipLength = AnimationExtension.GetAnimClipLength(_animator, clipName);
                yield return new WaitForSeconds(clipLength);
            }

            if (bombAreaPrefab)
            {
                var area = Instantiate(bombAreaPrefab, transform.position, Quaternion.identity);
                if (area.TryGetComponent(out BombArea bombArea))
                {
                    bombArea.Initialize(_damagePerSecond, _areaDuration, _areaRadius, _onAreaFinished);
                }
            }
            
            Destroy(gameObject);
        }
    }
}