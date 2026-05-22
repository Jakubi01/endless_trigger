using UnityEngine;

namespace UserInterface
{
    public class UserInterface : MonoBehaviour
    {
        public GameObject owner;

        private void Awake()
        {
            owner = gameObject;
        }
    }
}