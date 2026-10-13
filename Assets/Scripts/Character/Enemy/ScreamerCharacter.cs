using System.Collections;
using UnityEngine;

namespace Character.Enemy
{
    public class ScreamerCharacter : EnemyCharacterBase
    {
        // 0.5초마다 반경 4m 아군 이동속도 1.6배 버프
        [Header("Buff Settings")]
        [SerializeField] private float buffRadius = 4f;       // 반경 4m (요청사항 반영)
        [SerializeField] private float buffDuration = 3f;
        
        private readonly Collider2D[] _colliderBuffer = new Collider2D[99];
        private float _moveSpeedMultiplier = 1.6f;

        private void OnEnable()
        {
            OnAttackAction += OnAttack;
            OnDeathAction += OnDeath;
        }

        private void OnDisable()
        {
            OnAttackAction -= OnAttack;
            OnDeathAction -= OnDeath;
        }

        private void OnAttack()
        {
            ApplyBuffToAllies();
        }

        private void OnDeath()
        {
            
        }
        
        private void ApplyBuffToAllies()
        {
            if (!IsAlive) return;

            var count = Physics2D.OverlapCircleNonAlloc(transform.position, buffRadius, _colliderBuffer);
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var col = _colliderBuffer[i];
                if (!col) continue;

                if (!col.TryGetComponent(out EnemyCharacterBase enemy))
                    continue;
                
                enemy.StartBuffCoroutine(ApplySpeedBuff(enemy, _moveSpeedMultiplier, buffDuration));
            }
        }

        private IEnumerator ApplySpeedBuff(EnemyCharacterBase target, float multiplier, float duration)
        {
            target.moveSpeedMultiplier = multiplier;

            yield return new WaitForSeconds(duration);

            if (target && target.IsAlive)
            {
                target.moveSpeedMultiplier = 1.0f;
            }
        }
    }
}