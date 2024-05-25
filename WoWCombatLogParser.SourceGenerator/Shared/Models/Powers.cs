using System;
using System.Collections.Generic;
using System.Text;

namespace WoWCombatLogParser.Models;

public class Powers : CombatLogEventComponent
{
    public Soulbind Soulbind { get; set; }
    public Covenant Covenant { get; set; }
    [IsSingleDataField]
    public List<AnimaPower> AnimaPowers { get; set; }
    [IsSingleDataField]
    public List<SoulbindTrait> SoulbindTraits { get; set; }
    [IsSingleDataField]
    public List<Conduit> Conduits { get; set; }
}

[DebuggerDisplay("{Id} @ {Count} (Maw Power ID: {MawPowerId})")]
public class AnimaPower : CombatLogEventComponent
{
    public int Id { get; set; }
    public int MawPowerId { get; set; }
    public int Count { get; set; }
}

public class SoulbindTrait : IdPart<int>
{
}

[DebuggerDisplay("{Id} (Ilvl: {ItemLevel})")]
public class Conduit : IdPart<int>
{
    public int ItemLevel { get; set; }
}