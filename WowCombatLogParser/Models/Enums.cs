namespace WoWCombatLogParser.Models
{
    public enum FieldId
    {
        Timestamp = 0,
        EventType
    }

    public enum Reaction
    {
        Neutral,
        Friendly,
        Hostile
    }

    public enum Difficulty
    {
    }

    public enum UnitType
    {
        Player,
        Pet,
        NPC,
    }

    public enum EnvironmentalType
    {
        Drowning,
        Falling,
        Fatigue,
        Fire,
        Lava,
        Slime
    }

    public enum AuraType
    {
        BUFF,
        DEBUFF
    }

    public enum MissType
    {
        ABSORB,
        BLOCK,
        DEFLECT,
        DODGE,
        EVADE,
        IMMUNE,
        MISS,
        PARRY,
        REFLECT,
        RESIST
    }

    public enum Faction
    {
        Horde = 0,
        Alliance
    }

    public enum SpellSchool
    {
        None = 0,
        Physical = 0x1,
        Holy = 0x2,
        HolyStrike = Holy | Physical,
        Fire = 0x4,
        FlameStrike = Fire | Physical,
        Radiant = Holy | Fire,
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
        Astral = Arcane | Nature,
        Spellfrost = Arcane | Frost,
        Spellshadow = Arcane | Shadow,
        Chromatic = Spellshadow | Elemental,
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
