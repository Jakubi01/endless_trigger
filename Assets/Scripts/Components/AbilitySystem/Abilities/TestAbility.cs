using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "TestAbility", menuName = "Abilities/TestAbility")]
    public class TestAbility : AbilityBase
    {
        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            
            Debug.Log("TestAbility initialized");
        }

        public override bool Execute()
        {
            if (!base.Execute()) return false;
            
            // execute something..
            Debug.Log($"{Owner.name} has execute test ability.");

            return true;
        }
    }
}