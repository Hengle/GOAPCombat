using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Stamina : ITrait, IEquatable<Stamina>
    {
        public const string FieldStam = "Stam";
        public const string FieldAmount = "Amount";
        public AI.Planner.Domains.Enums.StatType Stam;
        public System.Int32 Amount;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Stam):
                    Stam = (AI.Planner.Domains.Enums.StatType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.StatType), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int32)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stamina.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(Stam):
                    return Stam;
                case nameof(Amount):
                    return Amount;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stamina.");
            }
        }

        public bool Equals(Stamina other)
        {
            return Stam == other.Stam && Amount == other.Amount;
        }

        public override string ToString()
        {
            return $"Stamina: {Stam} {Amount}";
        }
    }
}
