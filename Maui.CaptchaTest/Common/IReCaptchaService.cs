namespace Maui.CaptchaTest.Common
{
    public interface IReCaptchaService
    {
        Task<string> Verify(string siteKey, string domainUrl = "https://localhost");
        Task<bool> Validate(string captchaToken);
    }
}
