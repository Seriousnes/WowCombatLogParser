using System;
using System.Diagnostics;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.Models;

[DebuggerDisplay("{Id} {UnitName} {Flags} {RaidFlags}")]
public class Actor : Unit
{
    public UnitFlag Flags { get; set; }
    public RaidFlag RaidFlags { get; set; }
}

public class Unit : CombatLogEventComponent
{
    private string? _name;
    private string? _server;
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
    public string Name => _name ?? string.Empty;
    [NonData]
    public string Server => _server ?? string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is Unit unit && unit.Id == Id;
    }

    public override int GetHashCode() => base.GetHashCode();
}