﻿using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using WoWCombatLogParser;
using WoWCombatLogParser.Common.Models;

namespace WowCombatLogParser.App
{
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

            builder.Services.AddSingleton<IApplicationContext, ApplicationContext>();

            return builder.Build();
        }
    }
}