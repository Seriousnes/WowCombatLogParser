using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using WoWCombatLogParser;
using WoWCombatLogParser.IO;

namespace WowCombatLogParser.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ICombatLogEventMapper, CombatLogEventMapper>();
        builder.Services.AddSingleton<ICombatLogContextProvider, MemoryMappedCombatLogContextProvider>();
        builder.Services.AddSingleton<ICombatLogParser, CombatLogParser>();

        return builder.Build();
    }
}