using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Effects.DamageText
{
    public class FloatingText : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        private Canvas _canvas; // 캐싱용
        private IObjectPool<GameObject> _managedPool; // 반환용 풀 참조
        
        [SerializeField] private float moveSpeed = 2f;    // 위로 올라가는 속도
        [SerializeField] private float fadeSpeed = 3f;    // 사라지는 속도
        [SerializeField] private float duration = 0.5f;   // 유지 시간

        private void Awake()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas)
            {
                _canvas.worldCamera = Camera.main;
            }
        }

        public void Setup(string text, Color color, IObjectPool<GameObject> pool)
        {
            _managedPool = pool;
            _textMesh.text = text;
            
            Color c = color;
            c.a = 1f;
            _textMesh.color = c;

            StopAllCoroutines();
            StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeOutRoutine()
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                transform.Translate(Vector3.up * (moveSpeed * Time.deltaTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

            Color c = _textMesh.color;
            while (c.a > 0f)
            {
                transform.Translate(Vector3.up * (moveSpeed * Time.deltaTime));
                c.a -= fadeSpeed * Time.deltaTime;
                _textMesh.color = c;
                yield return null;
            }

            if (_managedPool != null)
            {
                _managedPool.Release(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}