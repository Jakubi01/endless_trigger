using UnityEngine;
using UserInterface;

using System;
using System.Collections.Generic;
using Character.Player;

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
        
        [SerializeField] private LevelUpSelectionWindow levelUpSelectionWindowPrefab;
        private LevelUpSelectionWindow _currentLevelUpSelectionWindow;

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

        /// <summary>
        /// 현재 사용 가능한 유효한 캔버스를 반환합니다.
        /// </summary>
        private Canvas GetActiveCanvas()
        {
            if (_mainCanvas != null) return _mainCanvas;
            
            _mainCanvas = FindFirstObjectByType<Canvas>();
            return _mainCanvas;
        }

        /// <summary>
        /// UI 윈도우 생성을 일반화한 제네릭 메서드
        /// </summary>
        private T OpenWindow<T>(T prefab, ref T currentWindowReference) where T : MonoBehaviour
        {
            if (currentWindowReference != null) return currentWindowReference;
            if (prefab == null) return null;

            Canvas activeCanvas = GetActiveCanvas();
            if (activeCanvas == null)
            {
                Debug.LogError("씬에 사용 가능한 Canvas가 없습니다.");
                return null;
            }

            currentWindowReference = Instantiate(prefab, activeCanvas.transform);
            currentWindowReference.transform.SetAsLastSibling();
            
            var rectTransform = currentWindowReference.GetComponent<RectTransform>();
            if (rectTransform) rectTransform.anchoredPosition = Vector2.zero;

            var canvasGroup = currentWindowReference.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = currentWindowReference.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.ignoreParentGroups = true;
            canvasGroup.interactable = true;

            RefreshPauseState();
            return currentWindowReference;
        }

#region TitleScene
        public void OnStartButtonClicked() => SceneControlManager.Instance.LoadScene(GameScene);
#endregion

#region Pause
        public void ShowPauseWindow() => OpenWindow(pauseWindowPrefab, ref _currentPauseWindow);

        public void OnContinueButtonClicked()
        {
            if (_currentPauseWindow == null) return;
            
            Destroy(_currentPauseWindow.gameObject);
            _currentPauseWindow = null;
            
            RefreshPauseState();
        }
#endregion

#region ExitWindow
        public void OnExitButtonClicked()
        {
            var win = OpenWindow(exitWindowPrefab, ref _currentExitWindow);
            if (win == null) return;

            var parentGroup = GetActiveCanvas().GetComponent<CanvasGroup>();
            if (parentGroup) parentGroup.interactable = false;
        }

        public void OnExitAccept()
        {
            Time.timeScale = 1f; 
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        public void OnExitCancel()
        {
            if (_currentExitWindow == null) return;
            
            var parentGroup = GetActiveCanvas().GetComponent<CanvasGroup>();
            if (parentGroup) parentGroup.interactable = true;
            
            Destroy(_currentExitWindow.gameObject);
            _currentExitWindow = null;
            
            RefreshPauseState();
        }
#endregion

#region SettingWindow
        public void OnSettingsButtonClicked() => OpenWindow(settingWindowPrefab, ref _currentSettingWindow);

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
        public void ShowGameOverWindow() => OpenWindow(gameOverWindowPrefab, ref _currentGameOverWindow);

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
            
            RefreshPauseState();
        }
#endregion

#region LevelUp
        public void ShowLevelUpSelection(IReadOnlyList<PlayerUpgradeDefinition> choices, Action<PlayerUpgradeDefinition> onSelected)
        {
            if (_currentLevelUpSelectionWindow || choices == null || choices.Count == 0) return;

            Canvas activeCanvas = GetActiveCanvas();
            if (!activeCanvas)
            {
                Debug.LogError("레벨업 선택 창을 표시할 Canvas가 없습니다.");
                return;
            }
            
            _currentLevelUpSelectionWindow = OpenWindow(levelUpSelectionWindowPrefab, ref _currentLevelUpSelectionWindow);
            _currentLevelUpSelectionWindow.Setup(choices, definition =>
            {
                LevelUpSelectionWindow currentWindow = _currentLevelUpSelectionWindow;
                _currentLevelUpSelectionWindow = null;
                if (currentWindow) Destroy(currentWindow.gameObject);
                RefreshPauseState();
                onSelected?.Invoke(definition);
            });
            
            _currentLevelUpSelectionWindow.transform.SetAsLastSibling();
            RefreshPauseState();
        }
#endregion

#region GoToTitle
        public void ShowGoToTitleWindow() => OpenWindow(goToTitleWindowPrefab, ref _currentGoToTitleWindow);
        
        public void OnGoToTitleButtonClicked()
        {
            ClearWindowReferences();
            
            Time.timeScale = 1f; 
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
            
            RefreshPauseState();
        }
#endregion

#region Shared
        private void ClearWindowReferences()
        {
            DestroyWindowIfAlive(_currentExitWindow);
            DestroyWindowIfAlive(_currentSettingWindow);
            DestroyWindowIfAlive(_currentPauseWindow);
            DestroyWindowIfAlive(_currentGameOverWindow);
            DestroyWindowIfAlive(_currentGoToTitleWindow);
            DestroyWindowIfAlive(_currentLevelUpSelectionWindow);

            _currentExitWindow = null;
            _currentSettingWindow = null;
            _currentPauseWindow = null;
            _currentGameOverWindow = null;
            _currentGoToTitleWindow = null;
            _currentLevelUpSelectionWindow = null;
        }

        private void DestroyWindowIfAlive(MonoBehaviour window)
        {
            if (window) Destroy(window.gameObject);
        }

        private void RefreshPauseState()
        {
            Time.timeScale = HasBlockingWindow() ? 0f : 1f;
        }

        private bool HasBlockingWindow()
        {
            return _currentExitWindow || _currentSettingWindow || _currentPauseWindow || _currentGameOverWindow || _currentGoToTitleWindow || _currentLevelUpSelectionWindow;
        }
        
        public void SetMainCanvas(Canvas mainCanvas)
        {
            _mainCanvas = mainCanvas;
        }
#endregion
    }
}
