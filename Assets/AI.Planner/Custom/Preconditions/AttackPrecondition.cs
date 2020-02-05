
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;


namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct AttackPrecondition : ICustomPrecondition<StateData>
    {




        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {

            var agentObjectIndex = originalState.GetTraitBasedObjectId(action[0]); //representing the first component of action "AGENT"
            var agentObject = originalState.GetTraitBasedObject(agentObjectIndex); //getting the game object here
            var aLocation = originalState.GetTraitOnObject<Location>(agentObject); //getting the trait of location using the agent object 
           

            var playerObjectIndex = originalState.GetTraitBasedObjectId(action[1]); //same as up there^
            var playerObject = originalState.GetTraitBasedObject(playerObjectIndex);
            var pLocation = originalState.GetTraitOnObject<Location>(playerObject);

            var distance = Vector3.Distance(aLocation.Position, pLocation.Position);

            //if the player is withing 15 ms away then i have seen the player 
           // Debug.Log(" IN ATTACK PRE" );
            if (distance <= 6)
            {
                //  Debug.Log(" in attack range" + 0);
                return true;

              
            }

           // Debug.Log("Not in attack range" + 1);
            return false;
        }



    }





#endif
}
