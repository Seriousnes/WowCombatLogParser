using System;

namespace WoWCombatLogParser.Models
{
    public readonly struct WowGuid : IEquatable<WowGuid>
    {
        public static readonly WowGuid Empty = new("0000000000000000");
        public string Value { get; }
        public bool IsEmpty => Value == Empty.Value;
        public bool IsPlayer => Value.StartsWith("Player-");
        public bool IsCreature => Value.StartsWith("Creature-");

        public WowGuid(string value)
        {
            Value = value;
        }

        public bool Equals(WowGuid other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is WowGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(WowGuid left, WowGuid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WowGuid left, WowGuid right)
        {
            return !left.Equals(right);
        }

        public static implicit operator WowGuid(string input)
        {
            return new WowGuid(input);
        }
    }
}
