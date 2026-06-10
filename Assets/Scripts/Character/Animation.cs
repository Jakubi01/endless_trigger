using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public static class AnimationExtension
    {
        public static float GetAnimClipLength(Animator animator, string animationName)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationName)
                    return clip.length;
            }

            return 1f;
        }

        public static float GetRandomizedClipDuration(Animator animator, string clipName, float variance)
        {
            return GetAnimClipLength(animator, clipName) + variance;
        }
    }
    
    public static class AnimatorParamToHash
    {
        // player
        public static readonly int IsRunning;
        public static readonly int Attack;

        // enemy
        public static readonly int Move;
        public static readonly int EnemyAttack;

        static AnimatorParamToHash()
        {
            IsRunning = Animator.StringToHash("IsRunning");
            Attack = Animator.StringToHash("Attack");
            
            Move = Animator.StringToHash("Move");
            EnemyAttack = Animator.StringToHash("EnemyAttack");
        }
    }
}