using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct EyeSight : ITrait, IEquatable<EyeSight>
    {
        public const string FieldPlayerSeen = "PlayerSeen";
        public const string FieldObjectSeen = "ObjectSeen";
        public System.Boolean PlayerSeen;
        public System.Boolean ObjectSeen;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(PlayerSeen):
                    PlayerSeen = (System.Boolean)value;
                    break;
                case nameof(ObjectSeen):
                    ObjectSeen = (System.Boolean)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait EyeSight.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(PlayerSeen):
                    return PlayerSeen;
                case nameof(ObjectSeen):
                    return ObjectSeen;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait EyeSight.");
            }
        }

        public bool Equals(EyeSight other)
        {
            return PlayerSeen == other.PlayerSeen && ObjectSeen == other.ObjectSeen;
        }

        public override string ToString()
        {
            return $"EyeSight: {PlayerSeen} {ObjectSeen}";
        }
    }
}
