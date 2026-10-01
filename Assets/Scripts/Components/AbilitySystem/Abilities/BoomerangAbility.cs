using System;
using Items.Projectile;
using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "BoomerangAbility", menuName = "Abilities")]
    public class BoomerangAbility : AbilityBase
    {
        [SerializeField] private float damage = 10f;

        private ProjectilePoolManager _projectilePoolManager;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            
            _projectilePoolManager = Owner.GetComponent<ProjectilePoolManager>();
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;

            Vector2 aimDir = Owner.transform.right;
            GameObject go = _projectilePoolManager.Spawn(Owner.transform.position, Quaternion.identity, aimDir, damage, pierceCount: 999);
           //  go.GetComponent<BoomerangProjectile>().Throw(Owner, aimDir);
            
            return true;
        }

        public override void Upgrade(float amount)
        {
            base.Upgrade(amount);
            
            
        }
    }
}