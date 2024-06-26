﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

public class Segment
{
    private readonly object _lock = new();
    private readonly string filename;
    private readonly long start;
    private readonly long length;
    private List<string>? lines;

    public Segment(string filename, long start, long length)
    {        
        this.filename = filename;
        this.start = start;
        this.length = length;
    }

    public List<string> Content
    {
        get
        {
            Load();
            return lines ?? [];
        }
    }

    public void Load()
    {
        lock(_lock)
        {
            if (lines != null) return;
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, StreamExtensions.GetBufferSize(filename), FileOptions.RandomAccess);
            fs.Seek(start, SeekOrigin.Begin);
            Span<byte> memory = new(new byte[(int)length]);
            fs.Read(memory);
            lines = Encoding.UTF8.GetString(memory.ToArray()).GetLines().ToList();
        }
    }
}