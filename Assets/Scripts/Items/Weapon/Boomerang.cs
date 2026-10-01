using UnityEngine;

namespace Items.Weapon
{
    public class Boomerang : MonoBehaviour
    {
        public float speed = 12f;
        public float maxDistance = 6f;
        public float spinSpeed = 720f;

        Transform owner;
        Vector2 dir;
        Vector2 startPos;
        bool returning;

        public void Throw(Transform owner, Vector2 direction)
        {
            this.owner = owner;
            dir = direction.normalized;
            startPos = transform.position;
        }

        void Update()
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            if (!returning)
            {
                transform.position += (Vector3)(dir * (speed * Time.deltaTime));
                if (Vector2.Distance(startPos, transform.position) >= maxDistance)
                    returning = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, owner.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, owner.position) < 0.1f)
                    Destroy(gameObject);
            }
        }
    }
}