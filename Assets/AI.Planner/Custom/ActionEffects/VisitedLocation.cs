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
    public struct VisitedLocation : ICustomActionEffect<StateData>
    {
        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {
            var agentLocationIndex = originalState.GetTraitBasedObjectId(action[0]);
            var agentLocation = originalState.GetTraitBasedObject(agentLocationIndex);
            var aLocation = originalState.GetTraitOnObject<Location>(agentLocation);

            var localIndex = originalState.GetTraitBasedObjectId(action[1]);
            var local = originalState.GetTraitBasedObject(localIndex);
            var visit = originalState.GetTraitOnObject<Local>(local);
            var location = originalState.GetTraitOnObject<Location>(local);

            var Distance = Vector3.Distance(aLocation.Position, location.Position);
            Debug.Log("Original State Location " + location.Position + " Visited "+  visit.Visited);
            if (visit.Visited == false )
            {
                visit.Visited = true;
                newState.SetTraitOnObject(visit, ref local);
                Debug.Log("New State Location " + location.Position + " Visited " + visit.Visited);
            }





        }
    }
}
#endif

