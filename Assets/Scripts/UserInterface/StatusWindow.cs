using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class StatusWindow : UserInterface
    {
        protected override void Awake()
        {
            base.Awake();
            
            var verticalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (!verticalLayoutGroup)
            {
                verticalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            verticalLayoutGroup.spacing = 0f;
            verticalLayoutGroup.padding = new RectOffset(20, 0, 0, 0);
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childScaleWidth = true;
            verticalLayoutGroup.childScaleHeight = true;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;
        }
    }
}