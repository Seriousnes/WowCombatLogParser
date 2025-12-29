using System;

namespace WoWCombatLogParser.Parser.EventMapping;

internal class SpellAbsorbedProfile : EventProfile
{
    public override Type EventType => typeof(SpellAbsorbed);

    public override CombatLogEventMapping GetMapping(CombatLogEventMapper mapper)
    {
        return (c, d, i) =>
        {
            SpellAbsorbed component = (SpellAbsorbed)c;

            component.Timestamp = GetValue<DateTime>(d[i++]);
            i = mapper.MapComponentAsProperties(component.Source!, d, i);
            i = mapper.MapComponentAsProperties(component.Destination!, d, i);

            /**
             * If there is additional spell info, i.e. the spell that was absorbed, the combatlog event line will 
             * contain 22 parameters; the typical 19, plus an additional 3 for the extra spell.
             */
            if (d.Count == 22)
            {
                i = mapper.MapComponentAsProperties(component.ExtraSpell!, d, i);
            }
            else
            {
                component.ExtraSpell = null;
            }

            i = mapper.MapComponentAsProperties(component.AbsorbedBy!, d, i);
            i = mapper.MapComponentAsProperties(component.Spell!, d, i);

            component.AbsorbedAmount = GetValue<int>(d[i++]);
            component.UnmitigatedAmount = GetValue<int>(d[i++]);

            return i;
        };
    }
}
