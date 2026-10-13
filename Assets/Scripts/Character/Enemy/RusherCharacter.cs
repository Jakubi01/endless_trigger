
namespace Character.Enemy
{
    public class RusherCharacter : EnemyCharacterBase
    {
        private void OnEnable()
        {
            OnAttackAction += OnAttackMotion;
        }

        private void OnDisable()
        {
            OnAttackAction -= OnAttackMotion;
        }

        private void OnAttackMotion()
        {
            
        }
    }
}