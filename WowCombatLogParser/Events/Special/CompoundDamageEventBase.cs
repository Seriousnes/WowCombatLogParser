using System.Collections.Generic;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Events
{
    public abstract class CompoundDamageEventBase<T> : CombatLogEvent<Spell, T> where T : EventSection, new()
    {
        public CompoundDamageEventBase(IEnumerable<IField> line = null) : base(line)
        {
        }

        public override EventBase BaseEvent { get; set; }
        public override Spell Prefix { get; } = new();
        public override T Suffix { get; } = new();
    }
}
