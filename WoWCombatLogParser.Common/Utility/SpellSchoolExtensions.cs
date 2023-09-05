using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Utility
{
    public static class SpellSchoolExtensions
    {
        public static bool Is(this SpellSchool spellSchool, params SpellSchool[] spellSchools) => spellSchools.CombineSpellSchools() == spellSchool;
        public static bool Is(this SpellSchool spellSchool, IEnumerable<SpellSchool> spellSchools) => spellSchool.Is(spellSchools.ToArray());
        public static bool Matches(this SpellSchool thisSpellSchool, SpellSchool other) => (thisSpellSchool | other) == thisSpellSchool;
        public static SpellSchool CombineSpellSchools(this IEnumerable<SpellSchool> spellSchools) => spellSchools.Aggregate(SpellSchool.None, (result, s) => result |= s);
        public static bool IsPhysical(this SpellSchool spellSchool) => spellSchool.Matches(SpellSchool.Physical);
        public static bool IsMagic(this SpellSchool spellSchool) => spellSchool > SpellSchool.Physical;
    }
}
