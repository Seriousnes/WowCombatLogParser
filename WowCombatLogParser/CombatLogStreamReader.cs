using System;
using System.IO;
using WoWCombatLogParser.Common.Models;
using static WoWCombatLogParser.IO.CombatLogFieldReader;

namespace WoWCombatLogParser;

internal class CombatLogStreamReader : IDisposable
{
    private readonly IApplicationContext _context;
    private FileStream? _file;
    private StreamReader? _reader;

    public CombatLogStreamReader(string filename, IApplicationContext applicationContext)
    {
        _context = applicationContext;
        SetFilename(filename);
    }

    public IEnumerable<CombatLogLineData> ReadLines()
    {
        string? line;
        while ((line = _reader?.ReadLine()) != null)
            yield return ReadFields(line);
    }
    
    public void SetFilename(string filename)
    {
        Close();
        _file = new FileStream(filename, new FileStreamOptions { Access = FileAccess.Read, Share = FileShare.ReadWrite });
        _reader = new StreamReader(_file);
        _context.EventGenerator = new EventGenerator() { ApplicationContext = _context };
        SetCombatLogVersion();
    }

    private void SetCombatLogVersion()
    {
        var version = _reader?.ReadLine();
        if (version != null)
            _context.EventGenerator.SetCombatLogVersion(version);
    }

    private void Close()
    {
        _reader?.Dispose();
        _file?.Dispose();
    }

    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }
}
