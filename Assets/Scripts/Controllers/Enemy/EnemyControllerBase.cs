using Character.Enemy;
using UnityEngine;

namespace Controllers.Enemy
{
    [RequireComponent(typeof(EnemyCharacterBase))]
    public abstract class EnemyControllerBase : Controller
    {
        protected EnemyCharacterBase EnemyCharacter;

        protected override void Awake()
        {
            base.Awake();
            EnemyCharacter = GetComponent<EnemyCharacterBase>();
        }
    }
}
