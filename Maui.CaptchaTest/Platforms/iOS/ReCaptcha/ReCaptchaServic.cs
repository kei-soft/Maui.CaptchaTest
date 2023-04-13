using Foundation;

using Maui.CaptchaTest.Common;

using Newtonsoft.Json;

using UIKit;

using WebKit;

namespace Maui.CaptchaTest.Platforms.iOS.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        private TaskCompletionSource<string> tcsWebView;
        private TaskCompletionSource<bool> tcsValidation;
        private ReCaptchaWebView reCaptchaWebView;

        public Task<bool> Validate(string captchaToken)
        {
            tcsValidation = new TaskCompletionSource<bool>();

            var captchaResult = new CaptchaResult();

            NSUrl url = new NSUrl(string.Format(Constants.ReCaptchaVerificationUrl, Constants.ReCaptchaSiteSecretKey, captchaToken));
            NSUrlRequest request = new NSUrlRequest(url);
            NSUrlSession session = null;
            NSUrlSessionConfiguration myConfig = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            myConfig.MultipathServiceType = NSUrlSessionMultipathServiceType.Handover;
            session = NSUrlSession.FromConfiguration(myConfig);
            NSUrlSessionTask task = session.CreateDataTask(request, (data, response, error) =>
            {
                Console.WriteLine(data);
                captchaResult = JsonConvert.DeserializeObject<CaptchaResult>(data.ToString());
                tcsValidation.TrySetResult(captchaResult.Success);
            });

            task.Resume();

            return tcsValidation.Task;
        }

        public Task<string> Verify(string siteKey, string domainUrl = "https://localhost")
        {
            tcsWebView = new TaskCompletionSource<string>();

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var webViewConfiguration = new WKWebViewConfiguration();
            reCaptchaWebView = new ReCaptchaWebView(window.Bounds, webViewConfiguration)
            {
                SiteKey = siteKey,
                DomainUrl = domainUrl
            };
            reCaptchaWebView.ReCaptchaCompleted += RecaptchaWebViewViewControllerOnReCaptchaCompleted;

#if DEBUG
            // Forces the Captcha Challenge to be explicitly displayed
            reCaptchaWebView.PerformSelector(new ObjCRuntime.Selector("setCustomUserAgent:"), NSThread.MainThread, new NSString("Googlebot/2.1"), true);
#endif

            reCaptchaWebView.CustomUserAgent = "Googlebot/2.1";

            window.AddSubview(reCaptchaWebView);
            reCaptchaWebView.LoadInvisibleCaptcha();

            return tcsWebView.Task;
        }

        private void RecaptchaWebViewViewControllerOnReCaptchaCompleted(object sender, string recaptchaResult)
        {
            if (!(sender is ReCaptchaWebView reCaptchaWebViewViewController))
            {
                return;
            }

            tcsWebView?.SetResult(recaptchaResult);
            reCaptchaWebViewViewController.ReCaptchaCompleted -= RecaptchaWebViewViewControllerOnReCaptchaCompleted;
            reCaptchaWebView.Hidden = true;
            reCaptchaWebView.StopLoading();
            reCaptchaWebView.RemoveFromSuperview();
            reCaptchaWebView.Dispose();
            reCaptchaWebView = null;
        }
    }
}
