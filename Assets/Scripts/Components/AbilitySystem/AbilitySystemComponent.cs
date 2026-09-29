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

/*  TODO
 * - Upgrade 카드가 만약 ability 카드라면
 * -- Abilities에 없음 : register new ability to Abilities 
 * -- Abilities에 있음 : ability upgrade execute
 * -> Select 카드는 캐릭터가 Ability를 가지고 있다면, Ability를 부여하는 카드를, 가지고 있지 않다면 Ability Upgrade 카드를 제시
 * ?? Select 카드는 캐릭터가 어떤 Ability를 가지고 있는지, 업그레이드 카드의 정보는 어디에 보관할것인지
 */

namespace Components.AbilitySystem
{
    public class AbilitySystemComponent : ComponentBase
    {
        public List<AbilityBase> Abilities { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            Abilities = true /* SaveData is valid? */ ? new List<AbilityBase>() : null;
        }

        private void Update()
        {
            if (Abilities.Count <= 0) return;

            foreach (var ability in Abilities)
            {
                ability.UpdateTimer(Time.deltaTime);
                ability.Execute();
            }
        }

        public void AddAbility(AbilityBase ability)
        {
            if (Abilities.Contains(ability)) return;

            var newAbility = Instantiate(ability);
            newAbility.Initialize(owner);
            Abilities.Add(newAbility);
        }
    }
}