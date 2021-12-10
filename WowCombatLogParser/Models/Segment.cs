﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("Event Count: {Events.Count}")]
    public class Segment
    {
        public Segment()
        {
        }

        public List<CombatLogEvent> Events { get; private set; } = new();

        public void ParseSegment(IList<CombatLogEvent> events)
        {
            Events.AddRange(events);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Parse();
            sw.Stop();

            Events = Events.OrderBy(i => i.Id).ToList();
            Console.WriteLine($"{Events.Count} processed in {sw.Elapsed.TotalSeconds}s");
        }        

        private void ParseParallel()
        {
            ParallelOptions po = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(Events, po, @event => @event.Parse());
        }

        private void Parse()
        {
            List<Task> tasks = new();
            Events.ForEach(i => tasks.Add(i.ParseAsync()));
            Task.WaitAll(tasks.ToArray());
        }
    }
}
