using System.Collections;
using Managers;
using UnityEngine;

namespace Character.Enemy.Boss
{
    public class BossClear : MonoBehaviour
    {
        private UIManager _uiManagerInstance;
        
        private void Awake()
        {
            _uiManagerInstance = UIManager.Instance;
        }
        
        public void StartGameClearEvent()
        { 
            StartCoroutine(ClearSequence());
        }

        private IEnumerator ClearSequence()
        {
            yield return new WaitForSeconds(2f);
            
            // Clear fx를 GameClear에 붙일까
            
            // TODO : 여기에 Clear fx?
            // The Dungeon King has fallen
            
            // TODO : 여기에 UI pop up
            _uiManagerInstance?.ShowGameClearWindow();
        }
    }
}