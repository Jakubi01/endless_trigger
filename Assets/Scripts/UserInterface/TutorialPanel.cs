using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class TutorialPanel : UserInterface
    {
        [SerializeField] private Button startButton;
        private SceneSpawnManager _sceneSpawnManager;
        
        protected override void Awake()
        {
            base.Awake();

            _sceneSpawnManager = FindFirstObjectByType<SceneSpawnManager>();
            
            if(_sceneSpawnManager)
            {
                startButton.onClick.AddListener(OnButtonClick);
                _sceneSpawnManager.SpawnCamera();
            }
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveAllListeners();
        }

        private void OnButtonClick()
        {
            if (!_sceneSpawnManager) return;

            _sceneSpawnManager.StartScene();
            Destroy(gameObject);
        }
    }
}