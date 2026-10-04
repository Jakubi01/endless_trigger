using System;
using UnityEngine;

namespace Components
{
    public class ComponentBase : MonoBehaviour
    {
        [NonSerialized] protected GameObject Owner;

        protected virtual void Awake()
        {
            Owner = gameObject;
        }
    }
}
