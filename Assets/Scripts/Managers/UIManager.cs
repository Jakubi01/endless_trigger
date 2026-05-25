using UnityEngine;
using UserInterface;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private ExitWindow exitWindowPrefab;
        private ExitWindow _currentExitWindow;

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
        
        
#region TitleScene
        public void OnStartButtonClicked()
        {
        }

        public void OnSettingsButtonClicked()
        {
        }

        public void OnExitButtonClicked()
        {
            ShowExitWindow();
        }

#endregion

#region SettingWindow



#endregion

#region ExitWindow
        private void ShowExitWindow()
        {
            if (_currentExitWindow) return;
            if (!exitWindowPrefab) return;

            var activeCanvas = FindActiveCanvasInScene();
            if (!activeCanvas) return;

            _currentExitWindow = Instantiate(exitWindowPrefab, activeCanvas.transform);
            _currentExitWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentExitWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private Canvas FindActiveCanvasInScene()
        {
            return FindFirstObjectByType<Canvas>();
        }

        public void OnExitAccept()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        public void OnExitReject()
        {
            if (_currentExitWindow == null) return;
            
            Destroy(_currentExitWindow.gameObject);
            _currentExitWindow = null;
        }
#endregion
    }
}