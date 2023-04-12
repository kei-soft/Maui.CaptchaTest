using System.Diagnostics;

using CoreGraphics;

using Foundation;

using Maui.CaptchaTest.Common;

using UIKit;

using WebKit;

namespace Maui.CaptchaTest.Platforms.iOS.ReCaptcha
{
    public sealed class ReCaptchaWebView : WKWebView, IWKScriptMessageHandler
    {
        private bool _captchaCompleted;
        public event EventHandler<string> ReCaptchaCompleted;

        public string SiteKey { get; set; }
        public string DomainUrl { get; set; }
        public string LanguageCode { get; set; }

        public ReCaptchaWebView(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration)
        {
            BackgroundColor = UIColor.Clear;
            ScrollView.BackgroundColor = UIColor.Clear;
            Opaque = false;
            Hidden = true;

            Configuration.UserContentController.AddScriptMessageHandler(this, "recaptcha");
        }

        public void LoadInvisibleCaptcha()
        {
            var html = new NSString(Constants.ReCaptchaHtml
                .Replace("${siteKey}", SiteKey));
            LoadHtmlString(html, new NSUrl(DomainUrl));
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            string post = message.Body.ToString();
            switch (post)
            {
                case "DidLoad":
                    ExecuteCaptcha();
                    break;
                case "ShowReCaptchaChallenge":
                    Hidden = false;
                    break;
                case "Error27FailedSetup":
                case "Error28Expired":
                case "Error29FailedRender":
                    if (_captchaCompleted)
                    {
                        OnReCaptchaCompleted(null);
                        Debug.WriteLine(post);
                        return;
                    }

                    _captchaCompleted = true; // 1 retry
                    Reset();
                    break;
                default:
                    if (post.Contains("ConsoleDebug:"))
                    {
                        Debug.WriteLine(post);
                    }
                    else
                    {
                        _captchaCompleted = true;
                        OnReCaptchaCompleted(post); // token
                    }
                    break;
            }
        }

        private void OnReCaptchaCompleted(string token)
        {
            ReCaptchaCompleted?.Invoke(this, token);
        }

        private async void ExecuteCaptcha()
        {
            await EvaluateJavaScriptAsync(new NSString("execute();"));
        }

        private async void Reset()
        {
            await EvaluateJavaScriptAsync(new NSString("reset();"));
        }
    }
}
