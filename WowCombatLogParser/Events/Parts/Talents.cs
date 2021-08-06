namespace WoWCombatLogParser.Events.Parts
{
    public class Talents : IEventSection
    {
        public int[] PvETalents { get; set; } = new int[6];
        public int[] PvPTalents { get; set; } = new int[4];
    }
}
