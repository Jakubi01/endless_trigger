using Character.Player;
using Managers;
using UnityEngine;

namespace Items.Exp
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExperiencePickup : MonoBehaviour
    {
        [SerializeField] private int amount = 5;
        [SerializeField] private float attractSpeed = 8f;

        private PlayerCharacter _player;
        private SoundManager _soundManager;
        private SceneSpawnManager _sceneSpawnManager;

        private void Awake()
        {
            var pickupCollider = GetComponent<CircleCollider2D>();
            pickupCollider.isTrigger = true;
            _soundManager = SoundManager.Instance;

            _sceneSpawnManager = FindFirstObjectByType<SceneSpawnManager>();
            if (!_sceneSpawnManager) return;

            _player = _sceneSpawnManager.Player;
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
            _soundManager.Play(SfxType.ItemGet);
            Destroy(gameObject);
        }
    }
}
