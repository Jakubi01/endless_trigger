namespace Character.Enemy
{
    public class TankerCharacter : EnemyCharacterBase
    {
        protected override void Awake()
        {
            base.Awake();

            SetProximityLimitRadius(1f);
        }
    }
}