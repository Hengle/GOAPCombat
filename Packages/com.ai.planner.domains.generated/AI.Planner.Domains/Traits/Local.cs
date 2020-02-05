using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Local : ITrait, IEquatable<Local>
    {
        public const string FieldVisited = "Visited";
        public System.Boolean Visited;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Visited):
                    Visited = (System.Boolean)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Local.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(Visited):
                    return Visited;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Local.");
            }
        }

        public bool Equals(Local other)
        {
            return Visited == other.Visited;
        }

        public override string ToString()
        {
            return $"Local: {Visited}";
        }
    }
}
