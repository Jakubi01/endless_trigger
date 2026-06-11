using Character;
using UnityEngine;

namespace Items.VFX
{
    [RequireComponent(typeof(Animator))]
    public class LevelUp : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (!_animator) return;

            const string clipName = "LevelUp";
            float length = AnimationExtension.GetAnimClipLength(_animator, clipName);
            if (length <= 0f) return;
            
            Destroy(gameObject, length);
        }
    }
}