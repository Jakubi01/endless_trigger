using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.Utilities
{
    public class DropdownDeselector : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(TrackBlockerDestroy());
        }

        private IEnumerator TrackBlockerDestroy()
        {
            yield return null;

            Transform blocker = null;
            const float timeout = 2.0f;
            var timer = 0f;

            while (!blocker && timer < timeout)
            {
                blocker = transform.root.Find("Blocker");
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!blocker) yield break;
            
            timer = 0f;
            while (blocker && timer < timeout)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            OnDropdownClose();
        }
        
        private void OnDropdownClose()
        {
            StartCoroutine(ClearFocusNextField());
        }

        private IEnumerator ClearFocusNextField()
        {
            yield return null;
        
            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}