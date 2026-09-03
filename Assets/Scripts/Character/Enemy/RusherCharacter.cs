namespace Character.Enemy
{
    public class RusherCharacter : EnemyCharacterBase
    {
        protected override void Awake()
        {
            base.Awake();

            SetProximityLimitRadius(.3f);
        }
    }
}