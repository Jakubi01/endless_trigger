using System.Collections.Generic;
using Character.Player;
using UnityEngine;

namespace Character.Enemy.Unused
{
    public class BlockerCharacter : EnemyCharacterBase
    {
        // 사망 시 반경 2.5m 폭발, 플레이어에게 35 데미지
        private readonly List<Collider2D> _colliderBuffer = new();
        private float _damage;
        
        protected override void Awake()
        {
            base.Awake();

            _damage = 35f;
        }
        
        protected override void Death()
        {
            base.Death();
            
            // 폭발 이펙트
            
            // find player 
            var count = Physics2D.OverlapCircle(transform.position, 1f, ContactFilter2D.noFilter , _colliderBuffer);
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var col = _colliderBuffer[i];
                if (!col) continue; 

                if (!col.TryGetComponent(out PlayerCharacter playerCharacter))
                    continue;
    
                playerCharacter.TakeDamage(_damage); 
                break;
            }
        }
    }
}