using Character.Player;
using UnityEngine;

namespace Items.Exp
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExperiencePickup : MonoBehaviour
    {
        [SerializeField] private int amount = 5;
        [SerializeField] private float attractSpeed = 8f;

        private PlayerCharacter _player;

        private void Awake()
        {
            CircleCollider2D pickupCollider = GetComponent<CircleCollider2D>();
            pickupCollider.isTrigger = true;
        }

        private void Update()
        {
            if (!_player)
            {
                _player = FindFirstObjectByType<PlayerCharacter>();
                if (!_player) return;
            }

            float distance = Vector2.Distance(transform.position, _player.transform.position);
            if (distance > _player.ExperiencePickupRange) return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                _player.transform.position,
                attractSpeed * Time.deltaTime
            );

            if (distance <= 0.2f)
            {
                Collect(_player);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCharacter player))
            {
                Collect(player);
            }
        }

        public void Initialize(int experienceAmount)
        {
            amount = experienceAmount;
        }

        private void Collect(PlayerCharacter player)
        {
            player.GainExperience(amount);
            Destroy(gameObject);
        }
    }
}
