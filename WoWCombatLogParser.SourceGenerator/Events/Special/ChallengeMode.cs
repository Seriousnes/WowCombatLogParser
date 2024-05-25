using System.Collections.Generic;
using WoWCombatLogParser.SourceGenerator.Events.Sections;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("CHALLENGE_MODE_START")]
internal abstract class ChallengeModeStart : CombatLogEventComponent, IFightStart
{
    public string ZoneName { get; set; }
    public int InstanceId { get; set; }
    public int ChallengeModeId { get; set; }
    public int KeystoneLevel { get; set; }
    [IsSingleDataField]
    public List<ChallengeModeAffix> Affixes { get; set; } = [];
}

[Affix("CHALLENGE_MODE_END")]
internal abstract class ChallengeModeEnd : CombatLogEventComponent, IFightEnd, IFightEndSuccess
{
    public int InstanceId { get; set; }
    public bool Success { get; set; }
    public int KeystoneLevel { get; set; }
    public int Duration { get; set; }
}

internal abstract class ChallengeModeAffix { }
