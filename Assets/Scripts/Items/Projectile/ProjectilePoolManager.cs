using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.Projectile
{
    public class ProjectilePoolManager : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxPoolSize = 50;

        private IObjectPool<GameObject> _pool;
        private readonly List<GameObject> _createdProjectiles = new();

        private void Awake()
        {
            _pool = new ObjectPool<GameObject>(
                createFunc: OnCreateProjectile,
                actionOnGet: OnGetProjectile,
                actionOnRelease: OnReleaseProjectile,
                actionOnDestroy: OnDestroyProjectile,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxPoolSize
            );
        }

        private void OnDestroy()
        {
            _pool?.Clear();
            _pool = null;

            foreach (GameObject projectile in _createdProjectiles)
            {
                if (projectile)
                {
                    Destroy(projectile);
                }
            }

            _createdProjectiles.Clear();
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, Vector2 direction, float damage = 0f, int pierceCount = 0
            , bool bSnapToParent = false, Transform snapTarget = null)
        {
            GameObject projectile = _pool.Get();

            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

            if (projectile.TryGetComponent(out Rigidbody2D rb))
            {
                rb.position = position;
                rb.rotation = rotation.eulerAngles.z;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            if (projectile.TryGetComponent(out Projectile projectileInstance))
            {
                projectileInstance.Initialize(direction, ReleaseProjectile, damage, pierceCount, bSnapToParent, snapTarget);
            }

            return projectile;
        }

        private GameObject OnCreateProjectile()
        {
            GameObject instance = Instantiate(projectilePrefab);
            _createdProjectiles.Add(instance);
            instance.SetActive(false);
            return instance;
        }

        private void OnGetProjectile(GameObject projectile)
        {
            projectile.SetActive(true);
        }

        private void OnReleaseProjectile(GameObject projectile)
        {
            projectile.SetActive(false);
        }

        private void OnDestroyProjectile(GameObject projectile)
        {
            Destroy(projectile);
        }

        private void ReleaseProjectile(GameObject projectile)
        {
            if (_pool == null || !projectile) return;

            _pool.Release(projectile);
        }
    }
}
