
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;


namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct ChasePrecondition: ICustomPrecondition<StateData>
    {




        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {
            var agentObjectIndex = originalState.GetTraitBasedObjectId(action[0]);
            var agentObject = originalState.GetTraitBasedObject(agentObjectIndex);
            var aLocation = originalState.GetTraitOnObject<Location>(agentObject);
            var aEyesight = originalState.GetTraitOnObject<EyeSight>(agentObject);

            var playerObjectIndex = originalState.GetTraitBasedObjectId(action[1]);
            var playerObject = originalState.GetTraitBasedObject(playerObjectIndex);
            var pLocation = originalState.GetTraitOnObject<Location>(playerObject);

            var distance = Vector3.Distance(aLocation.Position, pLocation.Position);


            // Debug.Log("Checking chase Precondition");
            // Debug.Log("chase Precondition Distance " + distance);
            if (aEyesight.PlayerSeen == true && distance < 20)
            {
                //Debug.Log("Returning true ");
                Debug.Log("chasing Distance " + distance);
                return true;
            }
            
           

            return false;
        }



    }
#endif
}