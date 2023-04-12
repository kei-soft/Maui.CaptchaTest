using Android.Content;
using Android.Gms.SafetyNet;

using Maui.CaptchaTest.Common;

using Newtonsoft.Json;

namespace Maui.CaptchaTest.Platforms.Android.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        readonly HttpClient _httpClient = new HttpClient();
        private static Context CurrentContext => Platform.CurrentActivity;

        private SafetyNetClient _safetyNetClient;
        private SafetyNetClient SafetyNetClient
        {
            get
            {
                return _safetyNetClient ??= SafetyNetClass.GetClient(CurrentContext);
            }
        }

        public async Task<string> Verify(string siteKey, string domainUrl = "https://localhost")
        {
            SafetyNetApiRecaptchaTokenResponse response = await SafetyNetClass.GetClient(Platform.CurrentActivity).VerifyWithRecaptchaAsync(siteKey);
            return response?.TokenResult;
        }

        public async Task<bool> Validate(string captchaToken)
        {
            var validationUrl = string.Format(Constants.ReCaptchaVerificationUrl, Constants.ReCaptchaSiteSecretKey, captchaToken);
            var response = await _httpClient.GetStringAsync(validationUrl);
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(response);
            return reCaptchaResponse.Success;
        }
    }
}
