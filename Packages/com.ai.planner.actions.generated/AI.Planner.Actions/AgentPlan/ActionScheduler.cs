using System;
using System.Collections.Generic;
using AI.Planner.Domains;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.AI.Planner.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace AI.Planner.Actions.AgentPlan
{
    public struct ActionScheduler :
        ITraitBasedActionScheduler<TraitBasedObject, StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>
    {
        public static readonly Guid ChaseTargetGuid = Guid.NewGuid();
        public static readonly Guid BasicAttackGuid = Guid.NewGuid();

        // Input
        public NativeList<StateEntityKey> UnexpandedStates { get; set; }
        public StateManager StateManager { get; set; }

        // Output
        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> IActionScheduler<StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>.CreatedStateInfo
        {
            set => m_CreatedStateInfo = value;
        }

        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> m_CreatedStateInfo;

        struct PlaybackECB : IJob
        {
            public ExclusiveEntityTransaction ExclusiveEntityTransaction;

            [ReadOnly]
            public NativeList<StateEntityKey> UnexpandedStates;
            public NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> CreatedStateInfo;
            public EntityCommandBuffer ChaseTargetECB;
            public EntityCommandBuffer BasicAttackECB;

            public void Execute()
            {
                // Playback entity changes and output state transition info
                var entityManager = ExclusiveEntityTransaction;

                ChaseTargetECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var ChaseTargetRefs = entityManager.GetBuffer<ChaseTargetFixupReference>(stateEntity);
                    for (int j = 0; j < ChaseTargetRefs.Length; j++)
                        CreatedStateInfo.Enqueue(ChaseTargetRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(ChaseTargetFixupReference));
                }

                BasicAttackECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var BasicAttackRefs = entityManager.GetBuffer<BasicAttackFixupReference>(stateEntity);
                    for (int j = 0; j < BasicAttackRefs.Length; j++)
                        CreatedStateInfo.Enqueue(BasicAttackRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(BasicAttackFixupReference));
                }
            }
        }

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var entityManager = StateManager.EntityManager;
            var ChaseTargetDataContext = StateManager.GetStateDataContext();
            var ChaseTargetECB = StateManager.GetEntityCommandBuffer();
            ChaseTargetDataContext.EntityCommandBuffer = ChaseTargetECB.ToConcurrent();
            var BasicAttackDataContext = StateManager.GetStateDataContext();
            var BasicAttackECB = StateManager.GetEntityCommandBuffer();
            BasicAttackDataContext.EntityCommandBuffer = BasicAttackECB.ToConcurrent();

            var allActionJobs = new NativeArray<JobHandle>(3, Allocator.TempJob)
            {
                [0] = new ChaseTarget(ChaseTargetGuid, UnexpandedStates, ChaseTargetDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [1] = new BasicAttack(BasicAttackGuid, UnexpandedStates, BasicAttackDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [2] = entityManager.ExclusiveEntityTransactionDependency
            };

            var allActionJobsHandle = JobHandle.CombineDependencies(allActionJobs);
            allActionJobs.Dispose();

            // Playback entity changes and output state transition info
            var playbackJob = new PlaybackECB()
            {
                ExclusiveEntityTransaction = StateManager.ExclusiveEntityTransaction,
                UnexpandedStates = UnexpandedStates,
                CreatedStateInfo = m_CreatedStateInfo,
                ChaseTargetECB = ChaseTargetECB,
                BasicAttackECB = BasicAttackECB,
            };

            var playbackJobHandle = playbackJob.Schedule(allActionJobsHandle);
            entityManager.ExclusiveEntityTransactionDependency = playbackJobHandle;

            return playbackJobHandle;
        }
    }
}
