using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("CHALLENGE_MODE_START")]
    public class ChallengeModeStart : EventSection, IFightStart
    {
        public string ZoneName { get; set; }
        public int InstanceId { get; set; }
        public int ChallengeModeId { get; set; }
        public int KeystoneLevel { get; set; }
        public EventSections<ChallengeModeAffix> Affixes { get; set; } = new EventSections<ChallengeModeAffix>();
    }

    [Affix("CHALLENGE_MODE_END")]
    public class ChallengeModeEnd : EventSection, IFightEnd, IFightEndSuccess
    {
        public int InstanceId { get; set; }
        public bool Success { get; set; }
        public int KeystoneLevel { get; set; }
        public int Duration { get; set; }
    }

    public class ChallengeModeAffix : IdPart
    {
    }
}
