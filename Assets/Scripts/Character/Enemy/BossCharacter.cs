using Character.Player;
using Components;
using Managers;
using UnityEngine;

namespace Character.Enemy
{
    public class BossCharacter : EnemyCharacterBase
    {
        private enum EBehaviourState
        {
            None,
            Idle,
            MoveToTarget,
            Combat,
            ExecutePattern,
            Dead
        }
        
        private EnemyBossCombatComponent _bossCombatComponent;
        private EBehaviourState _behaviourState = EBehaviourState.None;
        private PlayerCharacter _playerCharacter;

        protected override void Awake()
        {
            base.Awake();
            
            _bossCombatComponent = GetComponent<EnemyBossCombatComponent>();
            
            _playerCharacter = FindFirstObjectByType<PlayerCharacter>();
            if (!_playerCharacter)
            {
                var sceneSpawnManager = FindFirstObjectByType<SceneSpawnManager>();
                _playerCharacter = sceneSpawnManager?.Player;
                if(_playerCharacter)
                    _bossCombatComponent.SetTarget(_playerCharacter.transform);
            }

            // state machine에서 처리하기 때문에 event에서 제거
            OnAttackAction -= Attack;
        }

        private void Update()
        {
            if (!_playerCharacter)
            {
                ChangeState(EBehaviourState.None);
                return;
            }

            if (EnemyHealthComponent && !EnemyHealthComponent.IsAlive)
            {
                ChangeState(EBehaviourState.Dead);
                return;
            }

            if (_bossCombatComponent.IsExecutingPattern)
            {
                ChangeState(EBehaviourState.ExecutePattern);
                return;
            }

            var distance = Vector2.Distance(transform.position, _playerCharacter.transform.position);
            var nextState = distance <= AttackRadius
                ? EBehaviourState.Combat
                : EBehaviourState.MoveToTarget;

            ChangeState(nextState);
            SetAction();
        }
        
        private void ChangeState(EBehaviourState newState)
        {
            _behaviourState = newState;

            // 상태 진입 시 필요한 처리를 여기에 구현
        }

        private void SetAction()
        {
            switch(_behaviourState)
            {
                case EBehaviourState.Idle:
                case EBehaviourState.ExecutePattern:
                    break;
                
                case EBehaviourState.MoveToTarget:
                    MoveToTargetPosition(_playerCharacter.transform.position);
                    break;
                
                case EBehaviourState.Combat:
                    _bossCombatComponent.TryExecuteNextPattern(EnemyHealthComponent);
                    break;
                
                case EBehaviourState.None:
                default:
                    return;
            }
        }

        public override void TakeDamage(float amount)
        {
            // 여기에 무적 시간
            
            base.TakeDamage(amount);
        }
        
        protected override void Death()
        {
            base.Death();
            
            // Clear UI pop up
            _playerCharacter = null;
            _behaviourState = EBehaviourState.Idle;
        }
    }
}