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

            return 0f;
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
        public static readonly int PlayerDeath;

        // enemy
        public static readonly int Move;
        public static readonly int EnemyAttack;
        public static readonly int TakeDamage;
        public static readonly int Died;

        // Item
        public static readonly int Execute;
        
        static AnimatorParamToHash()
        {
            IsRunning = Animator.StringToHash("IsRunning");
            Attack = Animator.StringToHash("PlayerAttack");
            PlayerDeath = Animator.StringToHash("PlayerDeath");
            
            Move = Animator.StringToHash("Move");
            EnemyAttack = Animator.StringToHash("EnemyAttack");
            TakeDamage = Animator.StringToHash("TakeDamage");
            Died = Animator.StringToHash("Died");
            
            Execute = Animator.StringToHash("Execute");
        }
    }
}