using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{GetEncounterDescription()}")]
    public class Encounter
    {
        private static Comparison<CombatLogEvent> _comparison => (c1, c2) => c1.Id.CompareTo(c2.Id);
        private EncounterDetails details;
        private readonly List<CombatLogEvent> events = new List<CombatLogEvent>();

        public Encounter()
        {
        }

        public void Process(IList<CombatLogEvent> events)
        {
            this.events.AddRange(events);
            this.events.ForEach(@event => @event.Parse());
            this.events.Sort(_comparison);
        }

        public async Task ProcessAsync(IList<CombatLogEvent> events)
        {
            this.events.AddRange(events);
            await Parallel.ForEachAsync(this.events, async (@event, _) =>
            {
                await @event.ParseAsync();
            });
            this.events.Sort(_comparison);
        }

        public List<CombatLogEvent> Events => events;
        public EncounterDetails Details => details is null ? details = new(events) : details;      

        public string GetEncounterDescription()
        {
            var start = events.First() is CombatLogEvent<EncounterStart> first ? first : new CombatLogEvent<EncounterStart>();
            var end = events.Last();

            // final event may not be an encounter end event
            var (success, duration) = end is CombatLogEvent<EncounterEnd> encounterEnd ? (encounterEnd.Event.Success, TimeSpan.FromMilliseconds(encounterEnd.Event.FightTime)) : (false, end.BaseEvent.Timestamp - start.BaseEvent.Timestamp);

            return $"{start.Event.Name} {start.Event.DifficultyId.GetDifficultyInfo().Name }\n{(success ? "Kill" : "Wipe")} ({duration:m\\:ss})  {start.BaseEvent.Timestamp:h:mm tt}";
        }
    }


    public class EncounterDetails
    {
        private List<CombatLogEvent> events;

        public EncounterDetails(List<CombatLogEvent> events)
        {
            this.events = events;
            Process();
        }

        public List<CombatantDetails> Combatants { get; set; } = new List<CombatantDetails>();

        private void Process()
        {
            var actionEvents = events.OfType<ICompoundCombatLogEvent>().ToList();

            // setup combatants
            Combatants.AddRange(events.OfType<CombatLogEvent<CombatantInfo>>().Select(x => new CombatantDetails(x.Event)));
            Parallel.ForEach(Combatants, c => c.Name = actionEvents.Select(e => ((ComplexEventBase)e.BaseEvent).Source).FirstOrDefault(c => c.Id == c.Id)?.Name);
            // assign actions
            Parallel.ForEach(Combatants, c => c.Actions.AddRange(actionEvents.Where(x => ((ComplexEventBase)x.BaseEvent).Source.Id == c.Id).Cast<CombatLogEvent>().OrderBy(x => x.Id)));
        }
    }

    [DebuggerDisplay("{Name}")]
    public class CombatantDetails
    {
        private readonly CombatantInfo combatantInfo;

        public CombatantDetails(CombatantInfo combatantInfo, string name = "")
        {
            this.combatantInfo = combatantInfo;

            Stats = new Stats(combatantInfo);
            Equipment = new Equipment(combatantInfo);
            Name = name;
        }

        public WowGuid Id => combatantInfo.PlayerGuid;
        public string Name { get; set; }
        public Equipment Equipment { get; }
        public Stats Stats { get; }
        public List<CombatLogEvent> Actions { get; private set; } = new();
    }

    public class Stats
    {
        private readonly CombatantInfo combatantInfo;
        public Stats(CombatantInfo combatantInfo)
        {
            this.combatantInfo = combatantInfo;
        }

        public int Strength => combatantInfo.Strength;
        public int Agility => combatantInfo.Agility;
        public int Stamina => combatantInfo.Stamina;
        public int Intelligence => combatantInfo.Intelligence;
        public int Dodge => combatantInfo.Dodge;
        public int Parry => combatantInfo.Parry;
        public int Block => combatantInfo.Block;
        public int CritMelee => combatantInfo.CritMelee;
        public int CritRanged => combatantInfo.CritRanged;
        public int CritSpell => combatantInfo.CritSpell;
        public int Speed => combatantInfo.Speed;
        public int Lifesteel => combatantInfo.Lifesteel;
        public int HasteMelee => combatantInfo.HasteMelee;
        public int HasteRanged => combatantInfo.HasteRanged;
        public int HasteSpell => combatantInfo.HasteRanged;
        public int Avoidance => combatantInfo.Avoidance;
        public int Mastery => combatantInfo.Mastery;
        public int VersatilityDamageDone => combatantInfo.VersatilityDamageDone;
        public int VersatilityHealingDone => combatantInfo.VersatilityHealingDone;
        public int VersatilityDamageTaken => combatantInfo.VersatilityDamageDone;
        public int Armor => combatantInfo.Armor;
    }

    public class Equipment
    {
        private static readonly Dictionary<string, int> slotMap = new()
        {
            { "head", 1 },
            { "neck", 2 },
            { "shoulder", 3 },
            { "shirt", 4 },
            { "chest", 5 },
            { "waist", 6 },
            { "legs", 7 },
            { "feet", 8 },
            { "wrist", 9 },
            { "hands", 10 },
            { "finger1", 11 },
            { "finger2", 12 },
            { "trinket1", 13 },
            { "trinket2", 14 },
            { "back", 15 },
            { "mainhand", 16 },
            { "offhand", 17 },
            { "tabard", 18 },
        };
        private readonly CombatantInfo combatantInfo;

        public Equipment(CombatantInfo combatantInfo)
        {
            this.combatantInfo = combatantInfo;
        }

        public EquippedItem Ammo => getEquippedItem();
        public EquippedItem Head => getEquippedItem();
        public EquippedItem Neck => getEquippedItem();
        public EquippedItem Shoulder => getEquippedItem();
        public EquippedItem Shirt => getEquippedItem();
        public EquippedItem Chest => getEquippedItem();
        public EquippedItem Waist => getEquippedItem();
        public EquippedItem Legs => getEquippedItem();
        public EquippedItem Feet => getEquippedItem();
        public EquippedItem Wrist => getEquippedItem();
        public EquippedItem Hands => getEquippedItem();
        public EquippedItem Finger1 => getEquippedItem();
        public EquippedItem Finger2 => getEquippedItem();
        public EquippedItem Trinket1 => getEquippedItem();
        public EquippedItem Trinket2 => getEquippedItem();
        public EquippedItem Back => getEquippedItem();
        public EquippedItem MainHand => getEquippedItem();
        public EquippedItem OffHand => getEquippedItem();
        public EquippedItem Tabard => getEquippedItem();
        
        private EquippedItem getEquippedItem([CallerMemberName] string name = "")
        {
            if (slotMap.TryGetValue(name.ToLower(), out int index))
            {
                index--;
                if (combatantInfo.EquippedItems.Count >= index) return null;
                return combatantInfo.EquippedItems[index];
            }

            throw new ArgumentException($"{name} is an invalid equipment slot");
        }            
    }
}
