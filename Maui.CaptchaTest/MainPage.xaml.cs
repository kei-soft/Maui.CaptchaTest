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
            throw new Exception("Unable to retrieve reCaptcha Token");

        bool isValidCaptchaToken = await reCaptchaService.Validate(captchaToken);
        if (!isValidCaptchaToken)
            throw new Exception("reCaptcha token validation failed.");

        //#if ANDROID
        //        var api = Android.Gms.SafetyNet.SafetyNetClass.GetClient(Platform.CurrentActivity);
        //        var response = await api.VerifyWithRecaptchaAsync(AndroidSiteKey);
        //        if (response != null && !string.IsNullOrEmpty(response.TokenResult))
        //        {
        //            var captchaResponse = await ValidateCaptcha(response.TokenResult, AndroidSecretKey);
        //            if (captchaResponse is null || !captchaResponse.Success)
        //            {
        //                await Toast.Make($"Invalid captcha: {string.Join(",", captchaResponse?.ErrorCodes ?? Enumerable.Empty<object>())}", ToastDuration.Long).Show();
        //                return;
        //            }

        //            if (Platform.CurrentActivity!.PackageName != captchaResponse.ApkPackageName)
        //            {
        //                await Toast.Make($"Package Names do not match: {captchaResponse.ApkPackageName}", ToastDuration.Long).Show();
        //            }
        //            else
        //            {
        //                await Toast.Make("Success", ToastDuration.Long).Show();
        //            }
        //        }
        //        else
        //        {
        //            await Toast.Make("Failed", ToastDuration.Long).Show();
        //        }
        //#else
        //		await Toast.Make("This button works only on Android", ToastDuration.Long).Show();
        //#endif

    }
}

