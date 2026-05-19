using UnityEngine;
using UnityEngine.Pool;

namespace Items.Projectile
{
    public class ProjectilePoolManager : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxPoolSize = 50;

        private IObjectPool<Projectile> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Projectile>(
                createFunc: OnCreateProjectile,
                actionOnGet: OnGetProjectile,
                actionOnRelease: OnReleaseProjectile,
                actionOnDestroy: OnDestroyProjectile,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxPoolSize
            );
        }

        #region Pool Callbacks

        private Projectile OnCreateProjectile()
        {
            Projectile instance = Instantiate(projectilePrefab, transform);
            Debug.Log(instance);
            return instance;
        }

        private void OnGetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
        }

        private void OnReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyProjectile(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        #endregion

        public Projectile Spawn(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            Projectile projectile = _pool.Get();
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            
            projectile.Initialize(direction, ReleaseProjectile);
            
            return projectile;
        }

        private void ReleaseProjectile(Projectile projectile)
        {
            _pool.Release(projectile);
        }
    }
}