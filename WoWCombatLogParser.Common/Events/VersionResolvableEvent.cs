using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public interface IVersionResolvableEvent
    {
        CombatLogVersion Version { get; set; }
    }

    public interface IVersionResolvableEvent<T> : IVersionResolvableEvent
    {
    }    
}
