using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Models
{
    public interface ICombatLogEvent : IEventSection
    {
        int Id { get; }
        DateTime Timestamp { get; }
        string Event { get; }
        IList<IField> GetData(bool reset = true);
    }

    public interface ICombatantInfo
    {
        public WowGuid PlayerGuid { get; set; }
        public Faction Faction { get; set; }
    }

    public partial interface IFight
    {
    }

    /// <summary>
    /// CombatLogEvents with a source and destination
    /// </summary>
    public interface IAction
    {
        Unit Source { get; set; }
        Unit Destination { get; set; }
    }

    public interface IAbility
    {
        Ability Spell { get; set; }
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

    public interface IFightStart
    {
    }

    public interface IFightEnd
    {
        int Duration { get; }
    }

    public interface IFightEndSuccess : IFightEnd
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

    public interface IKey
    {
        bool EqualsKey(IKey key);
    }

    public partial interface IEventGenerator
    {
        IApplicationContext ApplicationContext { get; set; }
        CombatLogVersionEvent CombatLogVersionEvent { get; }

        T CreateEventSection<T>();
        T GetCombatLogEvent<T>(string line, Action<ICombatLogEvent> afterCreate = null) where T : class, ICombatLogEvent;
        ClassMap GetClassMap(Type type);
        void SetCombatLogVersion(string combatLogVersion);
        List<string> GetRegisteredEventHandlers();
        List<string> GetRegisteredClassMaps();
    }

    public interface ICombatLogParser
    {
        IApplicationContext ApplicationContext { get; set; }
        string Filename { get; set; }
        IEnumerable<IFight> Scan(bool quickScan = false);
        void Stop();
        void Watch(params FileSystemEventHandler[] fileChanged);
        void Parse(IFight encounter);
        void Parse(ICombatLogEvent combatLogEvent, IFight encounter = null);
        void Parse(IEnumerable<IFight> encounters);
        Task ParseAsync(ICombatLogEvent combatLogEvent, IFight encounter = null);
        Task ParseAsync(IFight encounter);
        Task ParseAsync(IEnumerable<IFight> encounters);
    }

    public interface IApplicationContext
    {
        ICombatLogParser CombatLogParser { get; set; }
        IEventGenerator EventGenerator { get; set; }
    }
}
