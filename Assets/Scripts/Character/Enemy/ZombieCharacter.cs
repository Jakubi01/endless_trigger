namespace Character.Enemy
{
    public class ZombieCharacter : EnemyCharacterBase
    {
        // Skeleton_Attack animation event. Keeping a type-specific entry point
        // makes the event receiver explicit in the Animation window.
        public void SkeletonAttackDamageEvent()
        {
            DamageToTarget();
        }
    }
}
