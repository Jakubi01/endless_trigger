using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneControlManager : MonoBehaviour
    {
        public static SceneControlManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}