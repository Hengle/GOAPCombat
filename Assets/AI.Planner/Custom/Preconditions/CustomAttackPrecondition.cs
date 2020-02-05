
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;


namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct CustomAttackPrecondition : ICustomPrecondition<StateData>
    {


       
        
        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {
            //Debug.Log("Checking lookAt Precondition");

             var DomianObjectBuffer = originalState.TraitBasedObjects;
             var agentObject = DomianObjectBuffer[action[0]];
             var attackObject = DomianObjectBuffer[action[1]];
           
             var locationBuffer = originalState.LocationBuffer;//getting all the locations in the state
             var agentLocation = locationBuffer[agentObject.LocationIndex]; //getting the location of the agent
             var targetLocation = locationBuffer[attackObject.LocationIndex]; // location of the target 

             var Distance = Vector3.Distance(agentLocation.Position, targetLocation.Position);

            //Debug.Log("Precondition Distance to target1 : " + Distance);
            //if the player is withing 15 ms away then i have seen the player 
            if (Distance <= 25)
             {
             //  Debug.Log("Precondition Distance to target : " + Distance);
                    return true;
                
                 
             }
            
            return false;
           
           
         }


           
    }
   



    
#endif
}
