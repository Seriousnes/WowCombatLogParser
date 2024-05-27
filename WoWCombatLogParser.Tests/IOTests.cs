using ByteSizeLib;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace WoWCombatLogParser.Tests;

public class IOTests(ITestOutputHelper output)
{
    private const string filename = @"TestLogs\Dragonflight\WoWCombatLog.txt";

    internal readonly ITestOutputHelper output = output;

    [Theory]
    [InlineData("Some test string with not a lot of data and some duplicate text in the string", "Some", 0)]
    [InlineData("Some test string with not a lot of data and some duplicate text in the string", "some duplicate", 44)]
    [InlineData("Some test string with not a lot of data and some duplicate text in the string", "Text not in the string", -1)]
    [InlineData("sosome", "some", 2)]
    public void Test_StreamExtensions_IndexOf_AsTheory(string source, string target, long expectedIndex)
    {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms) { AutoFlush = true };
        sw.Write(source);        
        ms.IndexOf(target, 0).Should().Be(expectedIndex);
    }

    [Theory]
    [InlineData("Some test string with not a lot of data\r\n and some duplicate text in the string", "string", 73)]
    [InlineData("Some test string with not a lot of data\r\n and some duplicate text in the string", " the", 68)]
    [InlineData("Some test string with not a lot of data\r\n and some duplicate text in the string", "Text not in the string", -1)]
    [InlineData(@"Some test string with not a lot of data
and some duplicate text in the string", "\r\n", 39, 41)]
    public void Test_StreamExtensions_LastIndexOf_AsTheory(string source, string target, long expectedIndex, long? startIndex = null)
    {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms) { AutoFlush = true };
        sw.Write(source);
        if (startIndex.HasValue)
            ms.LastIndexOf(target, startIndex.Value).Should().Be(expectedIndex);
        else
            ms.LastIndexOf(target).Should().Be(expectedIndex);
    }

    private readonly List<long> expectedStreamPositions = [8238838, 45390211, 46176415, 85248769, 91475221, 108816175, 139942758, 141175852, 158074360, 166786792, 183822650, 195043065, 213229974, 220542975, 239317560, 256811855, 282797912, 287168160, 295013244];
    private readonly List<int> expectedStringPositions = [8234486, 45362942, 46148845, 85194857, 91418153, 108748573, 139855516, 141087938, 157976424, 166683070, 183709022, 194922976, 213100654, 220410125, 239175042, 256660782, 282633375, 287001464, 294842850];

    [Fact]
    public void Test_StreamExtensions_IndexOf_On_File()
    {
        using var stream = new FileStream(
            filename,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Share = FileShare.ReadWrite,
                BufferSize = StreamExtensions.GetBufferSize(filename),
                Options = FileOptions.RandomAccess
            });
        var results = new List<long>();
        long i = -1;
        while ((i = stream.IndexOf("ENCOUNTER_START", i + 1)) >= 0)
            results.Add(i);
        results.Count.Should().Be(expectedStreamPositions.Count);
        results.Should().BeEquivalentTo(expectedStreamPositions);       
    }

    [Fact]
    public void Test_String_IndexOf_On_File()
    {
        var stopWatch = new Stopwatch();
        using var stream = new FileStream(
            filename,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Share = FileShare.ReadWrite,
                BufferSize = StreamExtensions.GetBufferSize(filename),
                Options = FileOptions.SequentialScan
            });
        using var sr = new StreamReader(stream);

        stopWatch.Start();
        var content = sr.ReadToEnd();
        stopWatch.Stop();
        output.WriteLine($"Reading file of {ByteSize.FromBytes(stream.Length)} took {stopWatch.ElapsedMilliseconds} ms");

        var results = new List<long>();
        int i = -1;
        stopWatch.Restart();
        while ((i = content.IndexOf("ENCOUNTER_START", i + 1)) >= 0)
            results.Add(i);
        stopWatch.Stop();
        output.WriteLine($"Finding {results.Count} instances of \"ENCOUNTER_START\" took: {stopWatch.ElapsedMilliseconds} ms");

        results.Count.Should().Be(expectedStringPositions.Count);
        results.Should().BeEquivalentTo(expectedStringPositions);
    }
}
