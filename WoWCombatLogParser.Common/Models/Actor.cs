using System;
using System.Diagnostics;
using System.Xml.Linq;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models;

[DebuggerDisplay("{Id} {UnitName} {Flags} {RaidFlags}")]
public class Actor : Unit
{
    public UnitFlag Flags { get; set; }
    public RaidFlag RaidFlags { get; set; }
}

public class Unit : CombatLogEventComponent, IKey
{
    private string _name;
    private string _server;
    public WowGuid Id { get; set; }
    public string UnitName
    {
        get => $"{_name}{(string.IsNullOrWhiteSpace(_server) ? "" : $"-{_server}")}";
        set
        {
            var values = value?.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < values?.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        _name = values[i].Trim();
                        break;
                    case 1:
                        _server = values[i].Trim();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [NonData]
    public string Name => _name;
    [NonData]
    public string Server => _server;

    public bool EqualsKey(IKey key)
    {
        return key is Unit unit && Id == unit.Id;
    }
}