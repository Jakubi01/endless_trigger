using System.Collections.Generic;
using Components.AbilitySystem.Abilities;
using UnityEngine;

/*
 * feature
 * - 각 능력을 보관, 실행
 * - 능력은 1회 실행하는 능력과, 매 프레임마다 타이머에 따라 실행되는 능력으로 구분됨
 * -- 1회 실행 -> effect
 * -- 타이머에 따라 실행 -> ability
 */

namespace Components.AbilitySystem
{
    public class AbilitySystem : ComponentBase
    {
        public List<AbilityBase> Abilities { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            Abilities = new List<AbilityBase>();
        }

        private void Update()
        {
            if (Abilities == null || Abilities.Count <= 0) return;

            for (int i = Abilities.Count - 1; i >= 0; i--)
            {
                AbilityBase ability = Abilities[i];
                if (!ability) continue;

                ability.UpdateTimer(Time.deltaTime);
                ability.Execute();
            }
        }

        public bool AddAbility(AbilityBase ability)
        {
            if (!ability || Abilities == null || HasAbility(ability.AbilityType)) return false;

            var newAbility = Instantiate(ability);
            newAbility.name = ability.name;
            newAbility.hideFlags = HideFlags.DontSave;
            newAbility.Initialize(owner);
            Abilities.Add(newAbility);
            return true;
        }
        
        public bool TryGetAbility(AbilityType type, out AbilityBase ability)
        {
            ability = Abilities?.Find(a => a && a.AbilityType == type);
            return ability;
        }

        public bool HasAbility(AbilityType type) => TryGetAbility(type, out _);

        private void OnDestroy()
        {
            if (Abilities == null) return;

            foreach (AbilityBase ability in Abilities)
            {
                if (ability)
                    Destroy(ability);
            }

            Abilities.Clear();
        }
    }
}
