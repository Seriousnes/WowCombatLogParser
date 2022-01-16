using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Events.Special
{
    [Affix("COMBATANT_INFO")]
    [DebuggerDisplay("{PlayerGuid} {Faction} {Strength} {Agility} {Stamina} {Intelligence} {Dodge} {Parry} {Block} {CritMelee} {CritRanged} {CritSpell} {Speed} {Lifesteel} {HasteMelee} {HasteRanged} {HasteSpell} {Avoidance} {Mastery} {VersatilityDamageDone} {VersatilityHealingDone} {VersatilityDamageTaken} {Armor} {CurrentSpecID} ")]
    public class CombatantInfo : Part
    {        
        public CombatantInfo()
        {
        }        

        public WowGuid PlayerGuid { get; set; }
        public Faction Faction { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Stamina { get; set; }
        public int Intelligence { get; set; }
        public int Dodge { get; set; }
        public int Parry { get; set; }
        public int Block { get; set; }
        public int CritMelee { get; set; }
        public int CritRanged { get; set; }
        public int CritSpell { get; set; }
        public int Speed { get; set; }
        public int Lifesteel { get; set; }
        public int HasteMelee { get; set; }
        public int HasteRanged { get; set; }
        public int HasteSpell { get; set; }
        public int Avoidance { get; set; }
        public int Mastery { get; set; }
        public int VersatilityDamageDone { get; set; }
        public int VersatilityHealingDone { get; set; }
        public int VersatilityDamageTaken { get; set; }
        public int Armor { get; set; }
        public int CurrentSpecID { get; set; }
        public Talents Talents { get; set; } = new();        
    }

    public class Talents : Part
    {
        private static readonly Regex _expr = new(@"\(?(\d+)\)?", RegexOptions.Compiled);
        public int[] PvETalents { get; set; } = new int[6];
        public int[] PvPTalents { get; set; } = new int[4];

        protected override void Parse(IEnumerator<string> data)
        {
            for (int i = 0; i < 6; i++)
            {
                data.MoveNext();
                PvETalents[i] = Convert.ToInt32(_expr.Replace(data.Current, "$1"));
            }

            for (int i = 0; i < 4; i++)
            {
                data.MoveNext();
                PvPTalents[i] = Convert.ToInt32(_expr.Replace(data.Current, "$1"));
            }
        }
    }

    public class Powers : Part
    {
        public Soulbind Soulbind { get; set; }
        public Covenant Covenant { get; set; }
    }
}
