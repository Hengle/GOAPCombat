using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Prey : ITrait, IEquatable<Prey>
    {

        public void SetField(string fieldName, object value)
        {
        }

        public object GetField(string fieldName)
        {
            throw new ArgumentException("No fields exist on trait Prey.");
        }

        public bool Equals(Prey other)
        {
            return true;
        }

        public override string ToString()
        {
            return $"Prey";
        }
    }
}
