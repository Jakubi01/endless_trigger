
namespace Character.Enemy
{
    public class RusherCharacter : EnemyCharacterBase
    {
        // 현재 구조는 AttackAction에 event를 구독시켜 Animation을 재생하는 방식인데,
        // 더 나은 방법은 Animation Event에 public void Foo()를 등록해서 
        // TryDamagePlayer => PlayAttackAnim() 실행
        // Foo => 데미지 처리
        // 이 구조가 훨씬 깔끔함
        // 대신 이 프로젝트에는 각 유닛마다 고유의 공격 애니메이션이 없음..
        // -> 공격 애니메이션을 기획이나 아트에서 만들어줘야 하는데 . . .
        
        // proximityLimitRadius = .3f;
        private void OnEnable()
        {
            AttackAction += AttackMotion;
        }

        private void OnDisable()
        {
            AttackAction -= AttackMotion;
        }

        private void AttackMotion()
        {
            
        }
    }
}