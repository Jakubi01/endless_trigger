using UnityEngine;
using UnityEngine.SceneManagement;
using UserInterface;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        public const string TitleScene = "TitleScene";
        public const string GameScene = "GameScene";

        [SerializeField] private ExitWindow exitWindowPrefab;
        private ExitWindow _currentExitWindow;
        
        [SerializeField] private SettingWindow settingWindowPrefab;
        private SettingWindow _currentSettingWindow;
        
        [SerializeField] private PauseWindow pauseWindowPrefab;
        private PauseWindow _currentPauseWindow;

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
            SceneControlManager.Instance.LoadScene(GameScene);
        }
#endregion


#region Pause
        public void ShowPauseWindow()
        {
            if (_currentPauseWindow) return;
            if (!pauseWindowPrefab) return;

            var activeCanvas = FindActiveCanvasInScene();
            if (!activeCanvas) return;

            _currentPauseWindow = Instantiate(pauseWindowPrefab, activeCanvas.transform);
            _currentPauseWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentPauseWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;

            TogglePause(true);
        }

        public void OnContinueButtonClicked()
        {
            if (_currentPauseWindow == null) return;
            TogglePause(false);
            
            Destroy(_currentPauseWindow.gameObject);
            _currentPauseWindow = null;
        }

        public void OnGoToTitleButtonClicked()
        {
            SceneControlManager.Instance.LoadScene(TitleScene);
        }

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

            TogglePause(true);
        }

        private Canvas FindActiveCanvasInScene()
        {
            return FindFirstObjectByType<Canvas>();
        }

        public void OnExitAccept()
        {
            TogglePause(false);
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        public void OnExitReject()
        {
            if (_currentExitWindow == null) return;
            TogglePause(false);
            
            Destroy(_currentExitWindow.gameObject);
            _currentExitWindow = null;
        }
#endregion

#region SettingWindow

        private void ShowSettingWindow()
        {
            if (_currentSettingWindow) return;
            if (!settingWindowPrefab) return;
            
            var activeCanvas = FindActiveCanvasInScene();
            if(!activeCanvas) return;
            
            _currentSettingWindow = Instantiate(settingWindowPrefab, activeCanvas.transform);
            _currentSettingWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentSettingWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;
            TogglePause(true);
        }

#endregion

#region Shared
        public void OnExitButtonClicked()
        {
            ShowExitWindow();
        }

        public void OnSettingsButtonClicked()
        {
            ShowSettingWindow();
        }
        
        private void TogglePause(bool pause)
        {
            var timeScale = pause ? 0f : 1f;
            Time.timeScale = timeScale;
        }
#endregion
    }
}