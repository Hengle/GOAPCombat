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
    public struct ChaseActionEffect : ICustomActionEffect<StateData>
    {
        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {
            var agentObjectIndex = originalState.GetTraitBasedObjectId(action[0]);
            var agentObject = originalState.GetTraitBasedObject(agentObjectIndex);
            var aLocation = originalState.GetTraitOnObject<Location>(agentObject);
            var aEyesight = originalState.GetTraitOnObject<EyeSight>(agentObject);
            var aStamina = originalState.GetTraitOnObject<Stamina>(agentObject);
            var bStamina = newState.GetTraitOnObject<Stamina>(agentObject);

            var playerObjectIndex = originalState.GetTraitBasedObjectId(action[1]);
            var playerObject = originalState.GetTraitBasedObject(playerObjectIndex);
            var pLocation = originalState.GetTraitOnObject<Location>(playerObject);

            var distance = Vector3.Distance(aLocation.Position, pLocation.Position);
            Debug.Log("Previous State stamina:" + aStamina.Amount);
            Debug.Log(" Current State stamina: " + bStamina.Amount);
           // Debug.Log("Chase State Distance: " + distance);
            




        }
    }
}
#endif