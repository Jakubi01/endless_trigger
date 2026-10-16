using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Enemy.Unused
{
    public class ScreamerCharacter : EnemyCharacterBase
    {
        // 0.5초마다 반경 4m 아군 이동속도 1.6배 버프
        [Header("Buff Settings")]
        [SerializeField] private float buffRadius = 4f;       // 반경 4m (요청사항 반영)
        [SerializeField] private float buffDuration = 3f;

        private readonly List<Collider2D> _colliderBuffer = new();
        private const float SpeedMultiplier = 1.6f;

        protected override void Attack()
        {
            base.Attack();
            
            ApplyBuffToAllies();
        }
        
        private void ApplyBuffToAllies()
        {
            if (!IsAlive) return;

            var count = Physics2D.OverlapCircle(transform.position, buffRadius, ContactFilter2D.noFilter, _colliderBuffer);
            if (count <= 0) return;
            
            for (var i = 0; i < count; i++)
            {
                var col = _colliderBuffer[i];
                if (!col) continue;

                if (!col.TryGetComponent(out EnemyCharacterBase enemy))
                    continue;
                
                enemy.StartBuffCoroutine(ApplySpeedBuff(enemy, SpeedMultiplier, buffDuration));
            }
        }

        private IEnumerator ApplySpeedBuff(EnemyCharacterBase target, float multiplier, float duration)
        {
            target.MoveSpeedMultiplier = multiplier;

            yield return new WaitForSeconds(duration);

            if (target && target.IsAlive)
            {
                target.MoveSpeedMultiplier = 1.0f;
            }
        }
    }
}