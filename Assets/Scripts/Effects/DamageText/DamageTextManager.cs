using UnityEngine;
using UnityEngine.Pool;

namespace Effects.DamageText
{
    public class DamageTextManager : MonoBehaviour
    {
        public static DamageTextManager Instance;

        [SerializeField] private GameObject textPrefab;
        private Transform _canvasTransform;

        private const int MaxSize = 200;

        private IObjectPool<GameObject> _pool;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _pool = new ObjectPool<GameObject>(
                CreateText,
                OnGetText,
                OnReleaseText,
                OnDestroyText,
                maxSize: MaxSize
            );

            if (!_canvasTransform)
            {
                _canvasTransform = FindFirstObjectByType<Canvas>().transform;
            }
        }

        private GameObject CreateText()
        {
            GameObject obj = Instantiate(textPrefab);
            obj.SetActive(false);
            return obj;
        }

        private void OnGetText(GameObject obj)
        {
            obj.SetActive(true);
        }

        private void OnReleaseText(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void OnDestroyText(GameObject obj)
        {
            Destroy(obj);
        }

        public void ShowDamageText(Vector3 worldPosition, string damageAmount, bool isCritical = false)
        {
            GameObject textObj = _pool.Get();

            Vector3 spawnPos = worldPosition + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0f);
            spawnPos.z = 0f;
            textObj.transform.position = spawnPos;

            FloatingText floatingText = textObj.GetComponent<FloatingText>();
            Color textColor = isCritical ? Color.red : Color.yellow;

            floatingText.Setup(damageAmount, textColor, _pool);
        }
    }
}