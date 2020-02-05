
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using System;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;

//precondition to trigger the navigate action

namespace AI.Planner.Actions.AgentPlan
{
#if PLANNER_DOMAINS_GENERATED
    public struct CustomNavigatePrecondition : ICustomPrecondition<StateData>
    {
        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {


            var agentLocationIndex = originalState.GetTraitBasedObjectId(action[0]);
            var agentLocation = originalState.GetTraitBasedObject(agentLocationIndex);
            var aLocation = originalState.GetTraitOnObject<Location>(agentLocation);


            var destinationLocationIndex = originalState.GetTraitBasedObjectId(action[1]);
            var destLocation = originalState.GetTraitBasedObject(destinationLocationIndex);
            var dLocation = originalState.GetTraitOnObject<Location>(destLocation);
            var visit = originalState.GetTraitOnObject<Local>(destLocation);

            /* var DomianObjectBuffer = originalState.TraitBasedObjects;
             var agentObject = DomianObjectBuffer[action[0]];
             var DestinationObject = DomianObjectBuffer[action[1]];

             var locationBuffer = originalState.LocationBuffer;//getting all the locations in the state
             var localBuffer = originalState.LocalBuffer;
             var agentLocation = locationBuffer[agentObject.LocationIndex]; //getting the location of the agent
             var destinationLocation = locationBuffer[DestinationObject.LocationIndex]; // location of the target 
             var visited = localBuffer[DestinationObject.LocalIndex];

             bool visit = visited.Visited;
           */
            var Distance = Vector3.Distance(aLocation.Position, dLocation.Position);
            Debug.Log("PRECON-Location: "+dLocation.Position + " Visited: "+ visit.Visited);

     //if distance isn't set the agent wont move on to the next position until the agent and destination locations are equal
     //That rarely happens so distance is needed plus it looks better
            if( Distance >= 2 && visit.Visited == false) 
            {
                return true;
            }

            if (visit.Visited == true)
            {
                return false;
            }
            return false;
        }

    }




#endif
}
