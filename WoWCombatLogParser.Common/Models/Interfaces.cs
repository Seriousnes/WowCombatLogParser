using System;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models
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
    public interface IAction
    {
        Unit Source { get; }
        Unit Destination { get; }
    }

    public interface IAbility
    {
        Ability Spell { get; }
    }

    public interface IDamageOrHealing
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
        SpellSchool School { get; }
        decimal Resisted { get; }
        decimal Blocked { get; }
        decimal Absorbed { get; }
        bool Critical { get; }
        bool Crushing { get; }
        bool IsOffHand { get; }
    }

    public interface IEncounterEnd
    {
        int Duration { get; }
    }

    public interface IEncounterSuccess
    {
        bool Success { get; }
    }

    public interface IAura
    {
        AuraType AuraType { get; }
    }

    public interface IDrain
    {
        PowerType PowerType { get; set; }
        decimal ExtraAmount { get; set; }
    }

    public interface IAdvancedDetails
    {
        WowGuid InfoGuid { get; }
        WowGuid OwnerGuid { get; }
        UnitInfo UnitInfo { get; }
        Location Location { get; }
        int Level { get; }
    }

    public interface IAdvancedLoggingDetails
    {
        AdvancedLoggingDetails AdvancedLoggingDetails { get; }
    }

    public interface ICast
    {
    }
}
