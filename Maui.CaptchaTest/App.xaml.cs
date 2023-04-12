using Maui.CaptchaTest.Common;

namespace Maui.CaptchaTest;

public partial class App : Application
{
    public App(IReCaptchaService reCaptchaService)
    {
        InitializeComponent();

        MainPage = new MainPage(reCaptchaService);
    }
}
