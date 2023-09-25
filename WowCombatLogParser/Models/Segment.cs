using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Models;

public class Segment
{
    private readonly object _lock = new object();
    private string filename;
    private long start;
    private long length;
    private List<string> lines;

    public Segment(string filename, long start, long length)
    {        
        this.filename = filename;
        this.start = start;
        this.length = length;
    }

    internal IParserContext ParserContext { get; set; }

    public List<string> Content
    {
        get
        {
            Parse();
            return lines;
        }
    }

    public void Parse()
    {
        lock(_lock)
        {
            if (lines != null) return;
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, StreamExtensions.GetBufferSize(filename), FileOptions.RandomAccess);
            fs.Seek(start, SeekOrigin.Begin);
            Span<byte> memory = new Span<byte>(new byte[(int)length]);
            fs.Read(memory);
            lines = Encoding.UTF8.GetString(memory.ToArray()).GetLines().ToList();
        }
    }
}