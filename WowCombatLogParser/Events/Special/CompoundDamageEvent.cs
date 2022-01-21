using System;
using System.Collections.Generic;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Events
{
    [Affix("DAMAGE_SPLIT")]
    public class DamageSplit : CompoundDamageEventBase<Damage> 
    {
        public DamageSplit(IEnumerable<IField> line = null) : base(line)
        {
        }
    }

    [Affix("DAMAGE_SHIELD")]
    public class DamageShield : CompoundDamageEventBase<Damage>
    {
        public DamageShield(IEnumerable<IField> line = null) : base(line)
        {
        }
    }

    [Affix("DAMAGE_SHIELD_MISSED")]
    public class DamageShieldMissed : CompoundDamageEventBase<Missed>
    {
        public DamageShieldMissed(IEnumerable<IField> line = null) : base(line)
        {
        }
    }    
}
