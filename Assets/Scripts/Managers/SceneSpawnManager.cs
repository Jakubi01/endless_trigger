using Character.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Managers
{
    public class SceneSpawnManager : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter playerPrefab;
        [SerializeField] private CinemachineCamera cinemachineCameraPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Collider2D cameraBoundingShape;
        [SerializeField] private bool disableSceneCameraTemplate = true;

        private PlayerCharacter _player;
        private CinemachineCamera _cinemachineCamera;

        private void Start()
        {
            SpawnPlayer();
            SpawnCamera();
            ConfigureCamera();
        }

        private void SpawnPlayer()
        {
            if (!playerPrefab)
            {
                Debug.LogError($"{nameof(SceneSpawnManager)}: Player prefab is not assigned.", this);
                return;
            }

            Vector3 spawnPosition = playerSpawnPoint ? playerSpawnPoint.position : Vector3.zero;
            _player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }

        private void SpawnCamera()
        {
            if (!cinemachineCameraPrefab)
            {
                Debug.LogError($"{nameof(SceneSpawnManager)}: Cinemachine camera prefab is not assigned.", this);
                return;
            }

            _cinemachineCamera = Instantiate(cinemachineCameraPrefab, Vector3.zero, Quaternion.identity);

            if (disableSceneCameraTemplate && cinemachineCameraPrefab.gameObject.scene.IsValid())
            {
                cinemachineCameraPrefab.gameObject.SetActive(false);
            }
        }

        private void ConfigureCamera()
        {
            if (!_player || !_cinemachineCamera) return;

            if (Camera.main != null && Camera.main.TryGetComponent(out CinemachineBrain cinemachineBrain))
            {
                cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.FixedUpdate;
            }

            _cinemachineCamera.Target.TrackingTarget = _player.transform;

            if (_cinemachineCamera.TryGetComponent(out CinemachineConfiner2D confiner))
            {
                confiner.BoundingShape2D = cameraBoundingShape;
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}
