using Character;
using Items.Projectile;
using Items.Weapon;
using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Boomerang Ability", menuName = "Endless Trigger/Abilities/Boomerang Ability")]
    public class BoomerangAbility : AbilityBase
    {
        /**
         * Execute: 부메랑 투척 후 캐릭터에게 돌아옴, 사거리 10m, 돌아오면 다시 투척 -> 쿨타임 x?
         * Upgrade: 발사 횟수 +1, 데미지 +20, 속도 증가 +10
         */
        
        private float _damage = 20f;

        private ProjectilePoolManager _projectilePoolManager;
        private CharacterBase _ownerChar;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);

            var boomerangAttack = Owner.transform.Find("BoomerangAttack");
            if (boomerangAttack)
            {
                _projectilePoolManager = boomerangAttack.GetComponent<ProjectilePoolManager>();
            }
            
            _ownerChar = Owner.GetComponent<CharacterBase>();
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            var nearestObject = _ownerChar.FindNearestFromCharacter(5f);
            var temp = (nearestObject.transform.position - Owner.transform.position).normalized;
            Vector2 aimDir = nearestObject ? temp.normalized : Owner.transform.forward; 
            GameObject go = _projectilePoolManager.Spawn(Owner.transform.position, Quaternion.identity, aimDir, _damage, pierceCount: 999);
            go.GetComponent<Boomerang>().Throw(Owner.transform, aimDir);
            
            return true;
        }

        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);

            _damage += 20f;
            Debug.Log("Boomerang Ability Upgraded");
        }
    }
}