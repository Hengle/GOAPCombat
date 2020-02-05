
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;


namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct CheckEyeSight: ICustomPrecondition<StateData>
    {



      
        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {
            //  Debug.Log("Checking Attack Precondition");

            var DomianObjectBuffer = originalState.TraitBasedObjects;
            var agentObject = DomianObjectBuffer[action[0]]; //agent object 
            

            var locationBuffer = originalState.LocationBuffer; //getting all the locations in the state
            var eyeSight = originalState.EyeSightBuffer; //getting the agents eyesight
            var agentLocation = locationBuffer[agentObject.LocationIndex]; //getting the location of the agent
            var agentSight = eyeSight[agentObject.EyeSightIndex]; //getting the agents eyesight

               
               
            Debug.Log("Player Not Detected " + 1);
            return false;
        }



    }





#endif
}