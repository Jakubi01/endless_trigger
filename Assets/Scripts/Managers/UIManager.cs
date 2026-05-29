using UnityEngine;
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
        public void OnStartButtonClicked() => SceneControlManager.Instance.LoadScene(GameScene);
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

            RefreshPauseState();
        }

        public void OnContinueButtonClicked()
        {
            if (_currentPauseWindow == null) return;
            
            Destroy(_currentPauseWindow.gameObject);
            _currentPauseWindow = null;
            RefreshPauseState();
        }

        public void OnGoToTitleButtonClicked()
        {
            ClearWindowReferences();
            RefreshPauseState();
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

            RefreshPauseState();
        }

        public void OnExitAccept()
        {
            ClearWindowReferences();
            RefreshPauseState();
            
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
            RefreshPauseState();
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
            RefreshPauseState();
        }

        public void OnSettingWindowClosed(SettingWindow settingWindow)
        {
            if (_currentSettingWindow == settingWindow)
            {
                Destroy(_currentSettingWindow.gameObject);
                _currentSettingWindow = null;
            }
            else if (settingWindow)
            {
                Destroy(settingWindow.gameObject);
            }

            RefreshPauseState();
        }

#endregion

#region Shared
        private Canvas FindActiveCanvasInScene() => FindFirstObjectByType<Canvas>();
        public void OnExitButtonClicked() => ShowExitWindow();
        public void OnSettingsButtonClicked() => ShowSettingWindow();

        private void ClearWindowReferences()
        {
            _currentExitWindow = null;
            _currentSettingWindow = null;
            _currentPauseWindow = null;
        }

        private void RefreshPauseState()
        {
            Time.timeScale = HasBlockingWindow() ? 0f : 1f;
        }

        private bool HasBlockingWindow()
        {
            return _currentExitWindow || _currentSettingWindow || _currentPauseWindow;
        }
#endregion
    }
}
