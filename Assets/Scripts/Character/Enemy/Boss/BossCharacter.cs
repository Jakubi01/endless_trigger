using Character.Player;
using Components;
using Managers;
using UnityEngine;

namespace Character.Enemy.Boss
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
        private BossClear _bossClear;

        protected override void Awake()
        {
            base.Awake();
            
            _bossCombatComponent = GetComponent<EnemyBossCombatComponent>();

            SetPlayerRef();

            IgnoreManager = true;
        }

        private void Start()
        {
            // state machine에서 처리하기 때문에 event에서 제거
            OnAttackAction -= Attack;

            _bossClear = FindAnyObjectByType<BossClear>();
        }

        private void Update()
        {
            if (!_playerCharacter)
            {
                ChangeState(EBehaviourState.None);
                SetPlayerRef();
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
            if (_behaviourState == newState) return;
            _behaviourState = newState;

            if (!EnemyNavMeshAgent || !EnemyNavMeshAgent.enabled) return;
            if (_behaviourState == EBehaviourState.MoveToTarget)
            {
                EnemyNavMeshAgent.isStopped = false;
            }
            else
            {
                EnemyNavMeshAgent.isStopped = true;
                EnemyNavMeshAgent.ResetPath();
            }
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

                case EBehaviourState.Dead:
                case EBehaviourState.None:
                default:
                    StopMove();
                    return;
            }
        }

        public override void TakeDamage(float amount)
        {
            if (!_bossCombatComponent || !_bossCombatComponent.TryReceiveDamage()) return;
            base.TakeDamage(amount);
        }
        
        protected override void Death()
        {
            base.Death();
            
            // Clear UI pop up
            _playerCharacter = null;
            _behaviourState = EBehaviourState.Idle;
            _bossClear?.StartGameClearEvent();
        }

        private void SetPlayerRef()
        {
            _playerCharacter = FindFirstObjectByType<PlayerCharacter>();
            if (_playerCharacter)
            {
                _bossCombatComponent.SetTarget(_playerCharacter.transform);
                return;
            }
            
            var sceneSpawnManager = FindFirstObjectByType<SceneSpawnManager>();
            _playerCharacter = sceneSpawnManager?.Player;
            if(_playerCharacter)
                _bossCombatComponent.SetTarget(_playerCharacter.transform);
        }
    }
}
