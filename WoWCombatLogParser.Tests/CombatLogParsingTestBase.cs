using System.Linq;
using Xunit.Abstractions;
using Xunit;
using System;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Tests;

public abstract class CombatLogParsingTestBase(ITestOutputHelper output)
{
    internal readonly ITestOutputHelper output = output;

    protected ICombatLogParser CombatLogParser = new CombatLogParser(new CombatLogEventMapper(), new MemoryMappedCombatLogContextProvider());

    internal void OutputEncounterSumary(IFight fight)
    {
        output.WriteLine($"CombatLogEventComponent Summary\n{new string('=', 35)}");
        output.WriteLine(fight.GetDetails().ToString());
        output.WriteLine(new string('-', 35));
        fight.GetEvents()
            .GroupBy(x => x.EventName)
            .OrderBy(x => x.Key)
            .ToList()
            .ForEach(x => output.WriteLine($"{x.Key,-25}{x.Count(),10}"));
        output.WriteLine(new string('-', 35));
        output.WriteLine($"{"Count",-11}{fight.GetEvents().Count,24}");
        output.WriteLine($"{new string('=', 35)}\n\n");
    }

    [Fact]
    public void OuputDamageTypes()
    {
        Enum.GetValues(typeof(SpellSchool))
            .Cast<SpellSchool>()
            .ToList()                
            .ForEach(x => output.WriteLine($"{x,-15} - {(int)x,3}"));
    }
}
