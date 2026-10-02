using UnityEngine;

namespace Items.Weapon
{
    public class Boomerang : MonoBehaviour
    {
        public float speed = 12f;
        public float maxDistance = 3f;
        public float spinSpeed = 720f;

        private Transform _owner;
        private Vector2 _dir;
        private Vector2 _startPos;
        private bool _returning;

        public void Throw(Transform owner, Vector2 direction)
        {
            _owner = owner;
            _dir = direction.normalized;
            _startPos = transform.position;
        }

        void Update()
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            if (!_returning)
            {
                transform.position += (Vector3)(_dir * (speed * Time.deltaTime));
                if (Vector2.Distance(_startPos, transform.position) >= maxDistance)
                    _returning = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _owner.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, _owner.position) < 0.1f)
                    Destroy(gameObject);
            }
        }
    }
}