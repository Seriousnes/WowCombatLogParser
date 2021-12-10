using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Simple
{
    
    [DebuggerDisplay("Encounter \"{Name}\" ({Id}) starting")]
    [Affix("ENCOUNTER_START")]
    public class EncounterStart : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DifficultyId { get; set; }
        public int GroupSize { get; set; }
        public int InstanceId { get; set; }        
    }

    [DebuggerDisplay("Encounter \"{Name}\" ({Id}) ended")]
    [Affix("ENCOUNTER_END")]
    public class EncounterEnd : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DifficultyId { get; set; }
        public int GroupSize { get; set; }
        public bool Success { get; set; }
    }


    [DebuggerDisplay("Loaded into map \"{Name}\" ({Id})")]
    [Affix("MAP_CHANGE")]
    public class MapChange : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [DebuggerDisplay("Loaded into zone \"{Name}\" ({Id})")]
    [Affix("ZONE_CHANGE")]
    public class ZoneChange : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DifficultyId { get; set; }
    }
}
