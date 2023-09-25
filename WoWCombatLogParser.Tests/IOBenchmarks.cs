using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Net.Mime;
using WoWCombatLogParser.IO;
using Xunit;

namespace WoWCombatLogParser.Tests;

[Config(typeof(Config))]
public class IOBenchmarks : IDisposable
{
    //private const string testString = "Some test string with not a lot of data and some duplicate text in the string";
    //private const string testValue = "some";
    private const string filename = @"TestLogs\Dragonflight\WoWCombatLog.txt";
    private Stream stream;
    private StreamReader streamReader;    

    private class Config : ManualConfig
    {
        public Config()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
            AddDiagnoser(ThreadingDiagnoser.Default);
            AddJob(Job.Dry.WithWarmupCount(1));
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        stream = new FileStream(
            filename,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Share = FileShare.ReadWrite,
                BufferSize = StreamExtensions.GetBufferSize(filename),
                Options = FileOptions.RandomAccess
            });        
        streamReader = new StreamReader(stream);
    }



    [Benchmark]
    public void StreamExtensions_IndexOf()
    {
        long i = -1;
        while ((i = stream.IndexOf("ENCOUNTER_START", i + 1)) >= 0)
        {
        }
    }

    [Benchmark]
    public void String_IndexOf()
    {
        var content = streamReader.ReadToEnd();
        int i = -1;
        while ((i = content.IndexOf("ENCOUNTER_START", i + 1)) >= 0)
        { 
        }
    }

    public void Dispose()
    {
        stream.Dispose();
        streamReader.Dispose();
        GC.SuppressFinalize(this);
    }
}


public class IOBenchmarkRunner
{
    [Fact]
    public void Run_Benchmarks()
    {
        var summary = BenchmarkRunner.Run<IOBenchmarks>();
    }
}