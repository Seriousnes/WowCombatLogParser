using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Parser.EventMapping;

public class SpellAbsorbedProfile : EventProfile
{
    public override Type EventType => typeof(SpellAbsorbed);

    public override CombatLogEventMapping GetMapping(CombatLogEventMapper mapper)
    {
        return (c, d, i) =>
        {
            var component = (SpellAbsorbed)c;

            component.Timestamp = Conversion.GetValue<DateTime>(d[i++]);
            i = mapper.MapFlatProperty(component.Source, d, i);
            i = mapper.MapFlatProperty(component.Destination, d, i);

            /**
             * If there is additional spell info, i.e. the spell that was absorbed, the combatlog event line will 
             * contain 22 parameters; the typical 19, plus an additional 3 for the extra spell.
             */
            if (d.Count == 22)
            {
                i = mapper.MapFlatProperty(component.ExtraSpell, d, i);
            }
            else
            {
                component.ExtraSpell = null;
            }

            i = mapper.MapFlatProperty(component.AbsorbedBy, d, i);
            i = mapper.MapFlatProperty(component.Spell, d, i);

            component.AbsorbedAmount = Conversion.GetValue<int>(d[i++]);
            component.UnmitigatedAmount = Conversion.GetValue<int>(d[i++]);

            return i;
        };
    }   
}
