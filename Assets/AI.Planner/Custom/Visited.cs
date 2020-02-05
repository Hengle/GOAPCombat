#if PLANNER_DOMAINS_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.Collections;
using Unity.AI.Planner.DomainLanguage.TraitBased;

namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct Visited : ICustomActionEffect<StateData>
    {
        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {

            var Location = newState.GetTraitBasedObjectId(action[1]); //getting local
            var localInfo = newState.GetTraitBasedObject(Location); 
            var visit = newState.GetTraitOnObject<Local>(localInfo);
            visit.Visited = true;
            newState.SetTraitOnObject(visit, ref localInfo);

            var Location1 = originalState.GetTraitBasedObjectId(action[1]); //getting local
            var localInfo1 = originalState.GetTraitBasedObject(Location1);
            var visit1 = originalState.GetTraitOnObject<Local>(localInfo1);
            visit1.Visited = true;
            newState.SetTraitOnObject(visit1, ref localInfo1);



        }
    }
}
#endif

