using Maui.CaptchaTest.Common;

namespace Maui.CaptchaTest;

public partial class MainPage : ContentPage
{
    IReCaptchaService reCaptchaService;

    public MainPage(IReCaptchaService reCaptchaService)
    {
        this.reCaptchaService = reCaptchaService;

        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var captchaToken = await reCaptchaService.Verify(Constants.ReCaptchaSiteKey);

        if (captchaToken == null)
        {
            throw new Exception("Unable to retrieve reCaptcha Token");
        }

        bool isValidCaptchaToken = await reCaptchaService.Validate(captchaToken);

        if (!isValidCaptchaToken)
        {
            throw new Exception("reCaptcha token validation failed.");

        }
    }
}

