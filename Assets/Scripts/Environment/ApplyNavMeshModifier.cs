using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Environment
{
    public class ApplyNavMeshModifier : MonoBehaviour
    {
        private void Awake()
        {
            var temp = new List<GameObject>();
            foreach (var child in transform)
            {
                if (temp.Contains((GameObject)child)) continue;
                temp.Add((GameObject)child);
            }

            if (temp.Count <= 0) return;

            foreach (var go in temp)
            {
                
            }
        }
    }
}