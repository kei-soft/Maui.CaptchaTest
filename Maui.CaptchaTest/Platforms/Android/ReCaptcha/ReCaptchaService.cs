using Android.Content;
using Android.Gms.SafetyNet;

using Maui.CaptchaTest.Common;

using Newtonsoft.Json;

namespace Maui.CaptchaTest.Platforms.Android.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        readonly HttpClient httpClient = new HttpClient();
        private static Context CurrentContext => Platform.CurrentActivity;

        private SafetyNetClient safetyNetClient;

        private SafetyNetClient SafetyNetClient
        {
            get
            {
                return safetyNetClient ??= SafetyNetClass.GetClient(CurrentContext);
            }
        }

        public async Task<string> Verify(string siteKey, string domainUrl = "https://localhost")
        {
            var client = SafetyNetClass.GetClient(Platform.CurrentActivity);

            SafetyNetApiRecaptchaTokenResponse response = await client.VerifyWithRecaptchaAsync(siteKey);

            return response?.TokenResult;
        }

        public async Task<bool> Validate(string captchaToken)
        {
            var validationUrl = string.Format(Constants.ReCaptchaVerificationUrl, Constants.ReCaptchaSiteSecretKey, captchaToken);
            var response = await httpClient.GetStringAsync(validationUrl);
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(response);

            return reCaptchaResponse.success;
        }
    }
}
