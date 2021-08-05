namespace WoWCombatLogParser.Events.Parts
{
    public class Unit : IEventSection
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public long Flags { get; set; }
        public long RaidFlags { get; set; }
    }
}
