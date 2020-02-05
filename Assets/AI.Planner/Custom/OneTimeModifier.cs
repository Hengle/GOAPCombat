using Unity.AI.Planner.DomainLanguage.TraitBased;
using UnityEngine;

#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;

namespace AI.Planner.Custom.Escape
{
    public class OneTimeModifier : ICustomTraitReward<Local>
    {
        public float RewardModifier(Local trait)
        {
            return (long)trait.GetField(Local.FieldVisited) > 0 ? -0.1f : 1;
        }
    }
}
#endif