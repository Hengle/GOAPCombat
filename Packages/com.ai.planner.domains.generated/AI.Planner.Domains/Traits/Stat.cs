using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Stat : ITrait, IEquatable<Stat>
    {
        public const string FieldStatType = "StatType";
        public const string FieldAmount = "Amount";
        public AI.Planner.Domains.Enums.StatType StatType;
        public System.Int32 Amount;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(StatType):
                    StatType = (AI.Planner.Domains.Enums.StatType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.StatType), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int32)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stat.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(StatType):
                    return StatType;
                case nameof(Amount):
                    return Amount;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stat.");
            }
        }

        public bool Equals(Stat other)
        {
            return StatType == other.StatType && Amount == other.Amount;
        }

        public override string ToString()
        {
            return $"Stat: {StatType} {Amount}";
        }
    }
}
