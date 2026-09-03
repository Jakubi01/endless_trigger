namespace Character.Enemy
{
    public class ZombieCharacter : EnemyCharacterBase
    {
        protected override void Awake()
        {
            base.Awake();

            SetProximityLimitRadius(.4f);
        }
    }
}