using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Player : ITrait, IEquatable<Player>
    {

        public void SetField(string fieldName, object value)
        {
        }

        public object GetField(string fieldName)
        {
            throw new ArgumentException("No fields exist on trait Player.");
        }

        public bool Equals(Player other)
        {
            return true;
        }

        public override string ToString()
        {
            return $"Player";
        }
    }
}
