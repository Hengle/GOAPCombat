﻿using System;
using Unity.Collections;
using UnityEngine;


namespace Unity.AI.Planner
{
    static class PolicyGraphExtensions
    {
        public static NativeHashMap<TStateKey, int> GetExpandedDepthMap<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo>(this PolicyGraph<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo> policyGraph, TStateKey rootKey)
            where TStateKey : struct, IEquatable<TStateKey>
            where TStateInfo : struct, IStateInfo
            where TActionKey : struct, IEquatable<TActionKey>
            where TActionInfo : struct, IActionInfo
            where TStateTransitionInfo : struct
        {
            var depthMap = new NativeHashMap<TStateKey, int>(policyGraph.StateInfoLookup.Length, Allocator.Persistent);

            var actionLookup = policyGraph.ActionLookup;
            var resultingStateLookup = policyGraph.ResultingStateLookup;
            var queue = new NativeQueue<(TStateKey, int)>(Allocator.TempJob);

            depthMap.TryAdd(rootKey, 0);
            queue.Enqueue((rootKey, 0));

            while (queue.TryDequeue(out var stateHorizonPair))
            {
                (var stateKey, int horizon) = stateHorizonPair;
                var nextHorizon = horizon + 1;

                if (actionLookup.TryGetFirstValue(stateKey, out var actionKey, out var iterator))
                {
                    do
                    {
                        var stateActionPair = new StateActionPair<TStateKey, TActionKey>(stateKey, actionKey);
                        if (resultingStateLookup.TryGetFirstValue(stateActionPair, out var resultingStateKey, out var resultIterator))
                        {
                            do
                            {
                                // Skip unexpanded states
                                if (!actionLookup.TryGetFirstValue(resultingStateKey, out _, out _))
                                    continue;

                                // first add will be most shallow due to BFS
                                if(depthMap.TryAdd(resultingStateKey, nextHorizon))
                                    queue.Enqueue((resultingStateKey, nextHorizon));

                            } while (resultingStateLookup.TryGetNextValue(out resultingStateKey, ref resultIterator));
                        }

                    } while (actionLookup.TryGetNextValue(out actionKey, ref iterator));
                }
            }

            queue.Dispose();

            return depthMap;
        }

        public static NativeHashMap<TStateKey, int> GetReachableDepthMap<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo>(this PolicyGraph<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo> policyGraph, TStateKey rootKey, Allocator allocator = Allocator.Persistent)
            where TStateKey : struct, IEquatable<TStateKey>
            where TStateInfo : struct, IStateInfo
            where TActionKey : struct, IEquatable<TActionKey>
            where TActionInfo : struct, IActionInfo
            where TStateTransitionInfo : struct
        {
            var depthMap = new NativeHashMap<TStateKey, int>(policyGraph.StateInfoLookup.Length, allocator);

            var actionLookup = policyGraph.ActionLookup;
            var resultingStateLookup = policyGraph.ResultingStateLookup;
            var queue = new NativeQueue<(TStateKey, int)>(Allocator.Temp);

            depthMap.TryAdd(rootKey, 0);
            queue.Enqueue((rootKey, 0));

            while (queue.TryDequeue(out var stateHorizonPair))
            {
                (var stateKey, int horizon) = stateHorizonPair;
                var nextHorizon = horizon + 1;

                if (actionLookup.TryGetFirstValue(stateKey, out var actionKey, out var iterator))
                {
                    do
                    {
                        var stateActionPair = new StateActionPair<TStateKey, TActionKey>(stateKey, actionKey);
                        if (resultingStateLookup.TryGetFirstValue(stateActionPair, out var resultingStateKey, out var resultIterator))
                        {
                            do
                            {
                                // first add will be most shallow due to BFS
                                if(depthMap.TryAdd(resultingStateKey, nextHorizon))
                                    queue.Enqueue((resultingStateKey, nextHorizon));

                            } while (resultingStateLookup.TryGetNextValue(out resultingStateKey, ref resultIterator));
                        }

                    } while (actionLookup.TryGetNextValue(out actionKey, ref iterator));
                }
            }

            queue.Dispose();

            return depthMap;
        }


#if !UNITY_DOTSPLAYER
        public static void LogStructuralInfo<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo>(this PolicyGraph<TStateKey, TStateInfo, TActionKey, TActionInfo, TStateTransitionInfo> policyGraph)
            where TStateKey : struct, IEquatable<TStateKey>, IComparable<TStateKey>
            where TStateInfo : struct, IStateInfo
            where TActionKey : struct, IEquatable<TActionKey>
            where TActionInfo : struct, IActionInfo
            where TStateTransitionInfo : struct
        {
            Debug.Log($"States: {policyGraph.StateInfoLookup.Length}");

            var (predecessorKeyArray, uniquePredecessorCount) = policyGraph.PredecessorGraph.GetUniqueKeyArray(Allocator.TempJob);
            Debug.Log($"States with Predecessors: {uniquePredecessorCount}");
            predecessorKeyArray.Dispose();

            var (stateActionKeyArray, uniqueStateActionCount) = policyGraph.ActionLookup.GetUniqueKeyArray(Allocator.TempJob);
            Debug.Log($"States with Successors: {uniqueStateActionCount}");
            stateActionKeyArray.Dispose();

            Debug.Log($"Actions: {policyGraph.ActionInfoLookup.Length}");
            Debug.Log($"Action Results: {policyGraph.StateTransitionInfoLookup.Length}");
        }
#endif
    }
}
