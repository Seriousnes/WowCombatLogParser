using System.Collections.Generic;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Affix("CHALLENGE_MODE_START")]
public class ChallengeModeStart : CombatLogEventComponent, IFightStart
{
    public string ZoneName { get; set; }
    public int InstanceId { get; set; }
    public int ChallengeModeId { get; set; }
    public int KeystoneLevel { get; set; }
    [IsSingleDataField]
    public List<ChallengeModeAffix> Affixes { get; set; } = [];
}

[Affix("CHALLENGE_MODE_END")]
public class ChallengeModeEnd : CombatLogEventComponent, IFightEnd, IFightEndSuccess
{
    public int InstanceId { get; set; }
    public bool Success { get; set; }
    public int KeystoneLevel { get; set; }
    public int Duration { get; set; }
}

public class ChallengeModeAffix : IdPart<ChallengeModeAffixEnum>
{
}
