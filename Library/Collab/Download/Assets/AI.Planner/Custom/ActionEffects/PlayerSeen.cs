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
    public struct PlayerSeen : ICustomActionEffect<StateData>
    {
        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {

            var eyeSightIndex = originalState.GetTraitBasedObjectId(action[0]); //same as when you're getting the preconditions 
            var eyeSight = originalState.GetTraitBasedObject(eyeSightIndex);
            var eyeTrait = originalState.GetTraitOnObject<EyeSight>(eyeSight);
          //  Debug.Log("Original State eyeSight: " + eyeTrait.PlayerSeen );

            if(eyeTrait.PlayerSeen == false)
            {
                eyeTrait.PlayerSeen = true; 
                newState.SetTraitOnObject(eyeTrait, ref eyeSight); //if you wanna change something you do it in NEWSTATE so now moving forward the change is applied. and (ref eyesight) you're refrencing the gameobject to change something on
            //    Debug.Log("New State eyeSight " + eyeTrait.PlayerSeen );
            }

            



        }
    }
}
#endif

