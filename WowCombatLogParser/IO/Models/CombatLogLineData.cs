namespace WoWCombatLogParser.IO;

internal class CombatLogLineData
{
    public CombatLogLineData(List<ICombatLogDataField> data)
    {
        if (data.Count > 1)
        {
            EventType = data[1].ToString()!;
            data.RemoveAt(1);
        }
        else
        {
            EventType = string.Empty;
        }
        Data = data;
    }

    public string EventType { get; set; }
    public List<ICombatLogDataField> Data { get; set; }
}