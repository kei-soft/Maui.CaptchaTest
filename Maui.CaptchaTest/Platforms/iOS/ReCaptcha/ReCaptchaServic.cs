using Foundation;

using Maui.CaptchaTest.Common;

using Newtonsoft.Json;

using UIKit;

using WebKit;

namespace Maui.CaptchaTest.Platforms.iOS.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        private TaskCompletionSource<string> _tcsWebView;
        private TaskCompletionSource<bool> _tcsValidation;
        private ReCaptchaWebView _reCaptchaWebView;

        public Task<bool> Validate(string captchaToken)
        {
            _tcsValidation = new TaskCompletionSource<bool>();

            var reCaptchaResponse = new ReCaptchaResponse();
            NSUrl url = new NSUrl(string.Format(Constants.ReCaptchaVerificationUrl, Constants.ReCaptchaSiteSecretKey, captchaToken));
            NSUrlRequest request = new NSUrlRequest(url);
            NSUrlSession session = null;
            NSUrlSessionConfiguration myConfig = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            myConfig.MultipathServiceType = NSUrlSessionMultipathServiceType.Handover;
            session = NSUrlSession.FromConfiguration(myConfig);
            NSUrlSessionTask task = session.CreateDataTask(request, (data, response, error) =>
            {
                Console.WriteLine(data);
                reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(data.ToString());
                _tcsValidation.TrySetResult(reCaptchaResponse.Success);
            });

            task.Resume();

            return _tcsValidation.Task;
        }

        public Task<string> Verify(string siteKey, string domainUrl = "https://localhost")
        {
            _tcsWebView = new TaskCompletionSource<string>();

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var webViewConfiguration = new WKWebViewConfiguration();
            _reCaptchaWebView = new ReCaptchaWebView(window.Bounds, webViewConfiguration)
            {
                SiteKey = siteKey,
                DomainUrl = domainUrl
            };
            _reCaptchaWebView.ReCaptchaCompleted += RecaptchaWebViewViewControllerOnReCaptchaCompleted;

#if DEBUG
            // Forces the Captcha Challenge to be explicitly displayed
            _reCaptchaWebView.PerformSelector(new ObjCRuntime.Selector("setCustomUserAgent:"), NSThread.MainThread, new NSString("Googlebot/2.1"), true);
#endif

            _reCaptchaWebView.CustomUserAgent = "Googlebot/2.1";

            window.AddSubview(_reCaptchaWebView);
            _reCaptchaWebView.LoadInvisibleCaptcha();

            return _tcsWebView.Task;
        }

        private void RecaptchaWebViewViewControllerOnReCaptchaCompleted(object sender, string recaptchaResult)
        {
            if (!(sender is ReCaptchaWebView reCaptchaWebViewViewController))
            {
                return;
            }

            _tcsWebView?.SetResult(recaptchaResult);
            reCaptchaWebViewViewController.ReCaptchaCompleted -= RecaptchaWebViewViewControllerOnReCaptchaCompleted;
            _reCaptchaWebView.Hidden = true;
            _reCaptchaWebView.StopLoading();
            _reCaptchaWebView.RemoveFromSuperview();
            _reCaptchaWebView.Dispose();
            _reCaptchaWebView = null;
        }
    }
}
