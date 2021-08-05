using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    public enum Reaction
    {
        Neutral,
        Friendly,
        Hostile
    }

    public enum UnitType
    {
        Player,
        Pet,
        NPC,
    }

    public enum SpellSchool
    {
        None = 0,
        Physical = 0x1,
        Holy = 0x2,
        HolyStrike = Holy | Physical,
        Fire = 0x4,
        FlameStrike = Fire | Physical,
        Holyfire = Holy | Fire,
        Nature = 0x8,
        Stormstrike = Nature | Physical,
        Holystorm = Nature | Holy,
        Firestorm = Nature | Fire,
        Frost = 0x10,
        Froststrike = Frost | Physical,
        Holyfrost = Frost | Holy,
        Frostfire = Frost | Fire,
        Froststorm = Frost | Nature,
        Elemental = Frost | Nature | Fire,
        Shadow = 0x20,
        Shadowstrike = Shadow | Physical,
        Twilight = Shadow | Holy,
        Shadowflame = Shadow | Fire,
        Plague = Shadow | Nature,
        Shadowfrost = Shadow | Frost,
        Arcane = 0x40,
        Spellstrike = Arcane | Physical,
        Divine = Arcane | Holy,
        Spellfire = Arcane | Fire,
        Spellstorm = Arcane | Nature,
        Spellfrost = Arcane | Frost,
        Spellshadow = Arcane | Shadow,
        Chromatic = Arcane | Shadow | Frost | Nature,
        Magic = Chromatic | Holy,
        Chaos = Magic | Physical
    }

    public enum PowerType
    {
        HealthCost = -2,
        None,
        Mana,
        Rage,
        Focus,
        Energy,
        ComboPoints,
        Runes,
        RunicPower,
        SoulShards,
        LunaPower,
        HolyPower,
        Alternate,
        Chi,
        Insanity,
        Obsolete,
        Obsolete2,
        ArcaneCharges,
        Fury,
        Pain,
        NumPowerTypes
    }
}
