using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class UserInterface : MonoBehaviour
    {
        public GameObject owner;

        protected virtual void Awake()
        {
            owner = gameObject;

            var canvas = GetComponentInParent<Canvas>();
            if (!canvas) return;

            var cs = canvas.GetComponent<CanvasScaler>();
            if(!cs) canvas.AddComponent<CanvasScaler>();

            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cs.matchWidthOrHeight = 0.5f;
            cs.referencePixelsPerUnit = 100f;
        }
        
        protected void SetToggleValue(Toggle toggle, bool value)
        {
            toggle.isOn = value;
        }
        
        protected void SetToggleText(in Toggle toggle, Text text)
        {
            text.text = toggle.isOn ? "ON" : "OFF";
        }
    }
}