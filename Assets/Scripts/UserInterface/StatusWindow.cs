using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class StatusWindow : UserInterface
    {
        protected override void Awake()
        {
            base.Awake();
            
            var verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            if (!verticalLayoutGroup)
            {
                verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            verticalLayoutGroup.spacing = 10f;
            verticalLayoutGroup.padding = new RectOffset(0, 0, 5, 5);
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childScaleWidth = true;
            verticalLayoutGroup.childScaleHeight = true;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
        }
    }
}