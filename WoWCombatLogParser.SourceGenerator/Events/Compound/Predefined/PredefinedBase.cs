﻿namespace WoWCombatLogParser.Common.Events;

public abstract class PredefinedBase : CombagLogEventComponent
{
}

public abstract class Predefined<T1> : PredefinedBase
{
}

public abstract class Predefined<T1, T2> : PredefinedBase
{
}
