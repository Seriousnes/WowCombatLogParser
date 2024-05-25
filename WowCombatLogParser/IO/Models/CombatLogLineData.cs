using System.Collections.Generic;

namespace WoWCombatLogParser.IO;

public sealed class CombatLogLineData
{
    public CombatLogLineData(List<ICombatLogDataField> data)
    {
        EventType = data[1].ToString();
        data.RemoveAt(1);
        Data = data;
    }

    public string EventType { get; set; }
    public List<ICombatLogDataField> Data { get; set; }
}