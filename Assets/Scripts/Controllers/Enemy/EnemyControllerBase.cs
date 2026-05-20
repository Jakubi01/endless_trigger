using Character.Enemy;
using Character.Player;
using UnityEngine;

namespace Controllers.Enemy
{
    public abstract class EnemyControllerBase : Controller
    {
        protected EnemyCharacterBase EnemyCharacter;
        protected static PlayerCharacter SharedTarget;

        protected override void Awake()
        {
            base.Awake();
            EnemyCharacter = GetComponent<EnemyCharacterBase>();
        }

        protected virtual void FixedUpdate()
        {
            if (!SharedTarget)
            {
                SharedTarget = FindFirstObjectByType<PlayerCharacter>();
            }

            if (!SharedTarget || !EnemyCharacter.IsAlive) return;

            EnemyCharacter.MoveToTargetPosition(SharedTarget.transform.position);
        }
    }
}
