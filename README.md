# WoWCombatLogParser

### A World of Warcraft combat log parser.

#
![](https://img.shields.io/badge/Version-Prerelease-critical?style=for-the-badge)

![](https://img.shields.io/badge/World%20of%20Warcraft-Dragonflight-orange?style=for-the-badge&color=9cf) 
![](https://img.shields.io/badge/Combat%20Log%20Version-20-informational?style=for-the-badge)
![](https://img.shields.io/badge/C%23-NET%207.0-success?style=for-the-badge)

[![Publish Package](https://github.com/Seriousnes/WowCombatLogParser/actions/workflows/publish_package.yml/badge.svg?branch=main)](https://github.com/Seriousnes/WowCombatLogParser/actions/workflows/publish_package.yml)

## How to use

### Dependency injection (recommended)

Register `ICombatLogParser` with a `CombatLogParser` implementation, plus its required dependencies (`ICombatLogEventMapper` and an `ICombatLogSegmentProvider`). All services should be registered with the same service lifetime.

```csharp
using Microsoft.Extensions.DependencyInjection;
using WoWCombatLogParser;
using WoWCombatLogParser.IO;

var services = new ServiceCollection();

services.AddTransient<ICombatLogEventMapper, CombatLogEventMapper>();
services.AddTransient<ICombatLogSegmentProvider, MemoryMappedCombatLogSegmentProvider>();
services.AddTransient<ICombatLogParser, CombatLogParser>();
```

The segment provider is required. You can swap `MemoryMappedCombatLogSegmentProvider` for `FileStreamCombatLogSegmentProvider` depending on your needs.

If you need more customization (for example, selecting the segment provider at runtime), register via an implementation factory:

```csharp
services.AddSingleton<ICombatLogEventMapper, CombatLogEventMapper>();

services.AddSingleton<ICombatLogSegmentProvider>(_ =>
{
	var useMemoryMapped = true;
	return useMemoryMapped
		? new MemoryMappedCombatLogSegmentProvider()
		: new FileStreamCombatLogSegmentProvider();
});

services.AddSingleton<ICombatLogParser>(sp => new CombatLogParser(
	sp.GetRequiredService<ICombatLogEventMapper>(),
	sp.GetRequiredService<ICombatLogSegmentProvider>()));
```

### Direct usage

```csharp
using WoWCombatLogParser;
using WoWCombatLogParser.IO;

var mapper = new CombatLogEventMapper();
var segmentProvider = new MemoryMappedCombatLogSegmentProvider();

using var parser = new CombatLogParser(mapper, segmentProvider);
parser.SetFilePath(@"C:\\Path\\To\\WoWCombatLog.txt");

foreach (var segment in parser.GetSegments())
{
	var eventsInSegment = await parser.ParseSegmentAsync(segment);
}
```

Process the returned events as needed. If parsing fails for some lines, `parser.Errors` is populated (keyed by combat log event type).
