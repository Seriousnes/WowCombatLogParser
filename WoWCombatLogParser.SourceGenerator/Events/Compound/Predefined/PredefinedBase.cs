namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Predefined;

internal abstract class PredefinedBase : CombatLogEventComponent
{
}

internal abstract class Predefined<T1> : PredefinedBase
{
}

internal abstract class Predefined<T1, T2> : PredefinedBase
{
}
