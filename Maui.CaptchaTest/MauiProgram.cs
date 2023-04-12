using CommunityToolkit.Maui;

using Maui.CaptchaTest.Common;

using Microsoft.Extensions.Logging;

namespace Maui.CaptchaTest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif



#if ANDROID
	    builder.Services.AddScoped<IReCaptchaService, Maui.CaptchaTest.Platforms.Android.ReCaptcha.ReCaptchaService>();
#elif IOS
        builder.Services.AddScoped<IReCaptchaService, Maui.CaptchaTest.Platforms.iOS.ReCaptcha.ReCaptchaService>();
#endif


        return builder.Build();
    }
}
