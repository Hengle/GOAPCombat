using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Stats : ITrait, IEquatable<Stats>
    {
        public const string FieldHp = "Hp";
        public const string FieldStamina = "Stamina";
        public System.Int64 Hp;
        public System.Int64 Stamina;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Hp):
                    Hp = (System.Int64)value;
                    break;
                case nameof(Stamina):
                    Stamina = (System.Int64)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stats.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(Hp):
                    return Hp;
                case nameof(Stamina):
                    return Stamina;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Stats.");
            }
        }

        public bool Equals(Stats other)
        {
            return Hp == other.Hp && Stamina == other.Stamina;
        }

        public override string ToString()
        {
            return $"Stats: {Hp} {Stamina}";
        }
    }
}
