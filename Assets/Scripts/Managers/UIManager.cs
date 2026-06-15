using UnityEngine;
using UserInterface;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas _mainCanvas;
        
        public const string TitleScene = "TitleScene";
        public const string GameScene = "GameScene";

        [SerializeField] private ExitWindow exitWindowPrefab;
        private ExitWindow _currentExitWindow;
        
        [SerializeField] private SettingWindow settingWindowPrefab;
        private SettingWindow _currentSettingWindow;
        
        [SerializeField] private PauseWindow pauseWindowPrefab;
        private PauseWindow _currentPauseWindow;
        
        [SerializeField] private GameOverWindow gameOverWindowPrefab;
        private GameOverWindow _currentGameOverWindow;
        
        [SerializeField] private GoToTitleWindow goToTitleWindowPrefab;
        private GoToTitleWindow _currentGoToTitleWindow;

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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            Time.timeScale = 1f;
        }
        
        
#region TitleScene
        public void OnStartButtonClicked() => SceneControlManager.Instance.LoadScene(GameScene);
#endregion


#region Pause
        public void ShowPauseWindow()
        {
            if (_currentPauseWindow) return;
            if (!pauseWindowPrefab) return;

            var activeCanvas = _mainCanvas;
            if (!activeCanvas) return;

            _currentPauseWindow = Instantiate(pauseWindowPrefab, activeCanvas.transform);
            _currentPauseWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentPauseWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;
            
            var windowCanvasGroup = _currentPauseWindow.GetComponent<CanvasGroup>();
            if (windowCanvasGroup == null)
            {
                windowCanvasGroup = _currentPauseWindow.gameObject.AddComponent<CanvasGroup>();
            }
            windowCanvasGroup.ignoreParentGroups = true;
            windowCanvasGroup.interactable = true;

            RefreshPauseState();
        }

        public void OnContinueButtonClicked()
        {
            if (_currentPauseWindow == null) return;
            
            var windowCanvasGroup = _currentPauseWindow.GetComponent<CanvasGroup>();
            windowCanvasGroup.ignoreParentGroups = true;
            windowCanvasGroup.interactable = true;
            
            Destroy(_currentPauseWindow.gameObject);
            _currentPauseWindow = null;
            RefreshPauseState();
        }
#endregion

#region ExitWindow
        private void ShowExitWindow()
        {
            if (_currentExitWindow) return;
            if (!exitWindowPrefab) return;

            var activeCanvas = _mainCanvas;
            if (!activeCanvas) return;

            _currentExitWindow = Instantiate(exitWindowPrefab, activeCanvas.transform);
            _currentExitWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentExitWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;

            activeCanvas.GetComponent<CanvasGroup>().interactable = false;
            
            var windowCanvasGroup = _currentExitWindow.GetComponent<CanvasGroup>();
            if (windowCanvasGroup == null)
            {
                windowCanvasGroup = _currentExitWindow.gameObject.AddComponent<CanvasGroup>();
            }
            windowCanvasGroup.ignoreParentGroups = true;
            windowCanvasGroup.interactable = true;
            
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

        public void OnExitCancel()
        {
            if (_currentExitWindow == null) return;
            
            var windowCanvasGroup = _currentExitWindow.GetComponent<CanvasGroup>();
            windowCanvasGroup.ignoreParentGroups = false;
            windowCanvasGroup.interactable = false;
            
            _mainCanvas.GetComponent<CanvasGroup>().interactable = true;
            
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

            var activeCanvas = _mainCanvas;
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

#region GameOver
        public void ShowGameOverWindow()
        {
            if (_currentGameOverWindow) return;
            if (!gameOverWindowPrefab) return;

            var activeCanvas = _mainCanvas;
            if(!activeCanvas) return;
            
            _currentGameOverWindow = Instantiate(gameOverWindowPrefab, activeCanvas.transform);
            _currentGameOverWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentGameOverWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;
            RefreshPauseState();
        }

        public void OnGameOverWindowClosed(GameOverWindow gameOverWindow)
        {
            if (_currentGameOverWindow == gameOverWindow)
            {
                Destroy(_currentGameOverWindow.gameObject);
                _currentGameOverWindow = null;
            }
            else if (gameOverWindow)
            {
                Destroy(gameOverWindow.gameObject);
            }
        }

#endregion

#region GoToTitle

public void ShowGoToTitleWindow()
        {
            if (_currentGoToTitleWindow) return;
            if (!goToTitleWindowPrefab) return;

            var activeCanvas = _mainCanvas;
            if(!activeCanvas) return;
            
            _currentGoToTitleWindow = Instantiate(goToTitleWindowPrefab, activeCanvas.transform);
            _currentGoToTitleWindow.transform.SetAsLastSibling();
            
            var rectTransform = _currentGoToTitleWindow.GetComponent<RectTransform>();
            if (!rectTransform) return;
            
            rectTransform.anchoredPosition = Vector2.zero;
            RefreshPauseState();
        }
        
        public void OnGoToTitleButtonClicked()
        {
            ClearWindowReferences();
            RefreshPauseState();
            SceneControlManager.Instance.LoadScene(TitleScene);
        }

        public void OnGoToTitleWindowClosed(GoToTitleWindow goToTitleWindow)
        {
            if(_currentGoToTitleWindow == goToTitleWindow)
            {
                Destroy(_currentGoToTitleWindow.gameObject);
                _currentGoToTitleWindow = null;
            }
            else if (goToTitleWindow)
            {
                Destroy(goToTitleWindow.gameObject);
            }
        }
#endregion

#region Shared
        public void OnExitButtonClicked() => ShowExitWindow();
        public void OnSettingsButtonClicked() => ShowSettingWindow();

        private void ClearWindowReferences()
        {
            DestroyWindowIfAlive(_currentExitWindow);
            DestroyWindowIfAlive(_currentSettingWindow);
            DestroyWindowIfAlive(_currentPauseWindow);
            DestroyWindowIfAlive(_currentGameOverWindow);
            DestroyWindowIfAlive(_currentGoToTitleWindow);

            _currentExitWindow = null;
            _currentSettingWindow = null;
            _currentPauseWindow = null;
            _currentGameOverWindow = null;
            _currentGoToTitleWindow = null;
        }

        private void DestroyWindowIfAlive(MonoBehaviour window)
        {
            if (window)
            {
                Destroy(window.gameObject);
            }
        }

        private void RefreshPauseState()
        {
            Time.timeScale = HasBlockingWindow() ? 0f : 1f;
        }

        private bool HasBlockingWindow()
        {
            return _currentExitWindow || _currentSettingWindow || _currentPauseWindow || _currentGameOverWindow || _currentGoToTitleWindow;
        }
        
        public void SetMainCanvas(Canvas mainCanvas)
        {
            _mainCanvas = mainCanvas;
        }
#endregion
    }
}
