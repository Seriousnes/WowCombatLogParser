using System;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models
{
    public interface ICombatLogEvent : IEventSection
    {
        int Id { get; }
        DateTime Timestamp { get; }
        string Event { get; }
        void Parse();
        Task ParseAsync();
    }

    /// <summary>
    /// CombatLogEvents with a source and destination
    /// </summary>
    public interface IAction : ICombatLogEvent
    {
        Unit Source { get; }
        Unit Destination { get; }
    }

    public interface ISpell : ICombatLogEvent
    {
        Ability Spell { get; }
    }

    public interface IDamageOrHealing : ICombatLogEvent
    {
        public decimal Amount { get; }
    }

    public interface IHealing : IDamageOrHealing
    {
        int Overhealing { get; }
        int Absorbed { get; }
        bool Critical { get; }
    }

    public interface IDamage : IDamageOrHealing
    {
        decimal UnmitigatedAmount { get; }
        bool IsOverkill { get; }
        SpellSchool School { get; }
        decimal Resisted { get; }
        decimal Blocked { get; }
        decimal Absorbed { get; }
        bool Critical { get; }
        bool Crushing { get; }
        bool IsOffHand { get; }
    }
 }
