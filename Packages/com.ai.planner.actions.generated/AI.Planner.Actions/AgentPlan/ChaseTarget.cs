using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Burst;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Actions.AgentPlan
{
    [BurstCompile]
    struct ChaseTarget : IJobParallelForDefer
    {
        public Guid ActionGuid;
        
        const int k_AgentIndex = 0;
        const int k_TowardsIndex = 1;
        const int k_MaxArguments = 2;

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        StateDataContext m_StateDataContext;

        internal ChaseTarget(Guid guid, NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            ActionGuid = guid;
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
        }

        public static int GetIndexForParameterName(string parameterName)
        {
            
            if (string.Equals(parameterName, "Agent", StringComparison.OrdinalIgnoreCase))
                 return k_AgentIndex;
            if (string.Equals(parameterName, "Towards", StringComparison.OrdinalIgnoreCase))
                 return k_TowardsIndex;

            return -1;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            var AgentFilter = new NativeArray<ComponentType>(4, Allocator.Temp){[0] = ComponentType.ReadWrite<AI.Planner.Domains.Agent>(),[1] = ComponentType.ReadWrite<AI.Planner.Domains.EyeSight>(),[2] = ComponentType.ReadWrite<Unity.AI.Planner.DomainLanguage.TraitBased.Location>(),[3] = ComponentType.ReadWrite<AI.Planner.Domains.Stamina>(),  };
            var TowardsFilter = new NativeArray<ComponentType>(2, Allocator.Temp){[0] = ComponentType.ReadWrite<AI.Planner.Domains.Player>(),[1] = ComponentType.ReadWrite<Unity.AI.Planner.DomainLanguage.TraitBased.Location>(),  };
            var AgentObjectIndices = new NativeList<int>(2, Allocator.Temp);
            stateData.GetTraitBasedObjectIndices(AgentObjectIndices, AgentFilter);
            var TowardsObjectIndices = new NativeList<int>(2, Allocator.Temp);
            stateData.GetTraitBasedObjectIndices(TowardsObjectIndices, TowardsFilter);
            var EyeSightBuffer = stateData.EyeSightBuffer;
            
            for (int i0 = 0; i0 < AgentObjectIndices.Length; i0++)
            {
                var AgentIndex = AgentObjectIndices[i0];
                var AgentObject = stateData.TraitBasedObjects[AgentIndex];
                
                if (!(EyeSightBuffer[AgentObject.EyeSightIndex].PlayerSeen == true))
                    continue;
            
            for (int i1 = 0; i1 < TowardsObjectIndices.Length; i1++)
            {
                var TowardsIndex = TowardsObjectIndices[i1];
                var TowardsObject = stateData.TraitBasedObjects[TowardsIndex];
                

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_AgentIndex] = AgentIndex,
                                                       [k_TowardsIndex] = TowardsIndex,
                                                    };
                argumentPermutations.Add(actionKey);
            }
            }
            AgentObjectIndices.Dispose();
            TowardsObjectIndices.Dispose();
            AgentFilter.Dispose();
            TowardsFilter.Dispose();
        }

        StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> ApplyEffects(ActionKey action, StateEntityKey originalStateEntityKey)
        {
            var originalState = m_StateDataContext.GetStateData(originalStateEntityKey);
            var originalStateObjectBuffer = originalState.TraitBasedObjects;

            var newState = m_StateDataContext.CopyStateData(originalState);
            {
                    new ChaseActionEffect().ApplyCustomActionEffectsToState(originalState, action, newState);
            }

            

            var reward = Reward(originalState, action, newState);
            var StateTransitionInfo = new StateTransitionInfo { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return new StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>(originalStateEntityKey, action, resultingStateKey, StateTransitionInfo);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = 5f;

            return reward;
        }

        public void Execute(int jobIndex)
        {
            m_StateDataContext.JobIndex = jobIndex; //todo check that all actions set the job index

            var stateEntityKey = m_StatesToExpand[jobIndex];
            var stateData = m_StateDataContext.GetStateData(stateEntityKey);

            var argumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            GenerateArgumentPermutations(stateData, argumentPermutations);

            var transitionInfo = new NativeArray<ChaseTargetFixupReference>(argumentPermutations.Length, Allocator.Temp);
            for (var i = 0; i < argumentPermutations.Length; i++)
            {
                transitionInfo[i] = new ChaseTargetFixupReference { TransitionInfo = ApplyEffects(argumentPermutations[i], stateEntityKey) };
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<ChaseTargetFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(transitionInfo);

            transitionInfo.Dispose();
            argumentPermutations.Dispose();
        }

        
        public static T GetAgentTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_AgentIndex]);
        }
        
        public static T GetTowardsTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_TowardsIndex]);
        }
        
    }

    public struct ChaseTargetFixupReference : IBufferElementData
    {
        internal StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> TransitionInfo;
    }
}


