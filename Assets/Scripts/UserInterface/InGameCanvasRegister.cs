using Managers;
using UnityEngine;

namespace UserInterface
{
    public class InGameCanvasRegister : MonoBehaviour
    {
        private void Start()
        {
            if (!UIManager.Instance) return;
            
            Canvas myCanvas = GetComponent<Canvas>();
            UIManager.Instance.SetMainCanvas(myCanvas);
        }
    }
}