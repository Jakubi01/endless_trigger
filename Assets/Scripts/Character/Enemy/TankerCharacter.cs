namespace Character.Enemy
{
    public class TankerCharacter : EnemyCharacterBase
    {
        // GolemB_Attack animation event. Keeping a type-specific entry point
        // makes the event receiver explicit in the Animation window.
        public void GolemBAttackDamageEvent()
        {
            DamageToTarget();
        }
    }
}
