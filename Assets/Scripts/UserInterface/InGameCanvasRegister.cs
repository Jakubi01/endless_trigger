using Managers;
using UnityEngine;

namespace UserInterface
{
    public class InGameCanvasRegister : MonoBehaviour
    {
        private void Start()
        {
            if (UIManager.Instance == null) return;
            
            Canvas myCanvas = GetComponent<Canvas>();
            UIManager.Instance.SetMainCanvas(myCanvas);
        }
    }
}