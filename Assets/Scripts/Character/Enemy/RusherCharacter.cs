
namespace Character.Enemy
{
    public class RusherCharacter : EnemyCharacterBase
    {
        // SlimeLit_Attack animation event. Keeping a type-specific entry point
        // makes the event receiver explicit in the Animation window.
        public void SlimeLitAttackDamageEvent()
        {
            DamageToTarget();
        }
    }
}
