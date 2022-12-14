using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [CombatLogVersion(CombatLogVersion.Wotlk)]
    [Affix("COMBATANT_INFO")]
    [DebuggerDisplay("{PlayerGuid} {Faction}")]
    public class WotlkCombatantInfo : CombatantInfo, ICombatantInfo
    {
    }
}
