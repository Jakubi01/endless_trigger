using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.Utilities
{
    public class DropdownDeselector : MonoBehaviour, IPointerClickHandler
    {
        private Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<Dropdown>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(TrackBlockerDestroy());
        }

        private IEnumerator TrackBlockerDestroy()
        {
            yield return null;
            
            var blocker = transform.root.Find("Blocker");
            if (!blocker)
            {
                Debug.LogWarning("Blocker not found");
                yield break;
            }

            while (blocker)
            {
                yield return null;
            }

            OnDropdownClose();
        }
        
        private void OnDropdownClose()
        {
            StartCoroutine(ClearFocusNextFrame());
        }

        private IEnumerator ClearFocusNextFrame()
        {
            yield return null;
        
            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}