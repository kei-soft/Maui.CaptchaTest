namespace Maui.CaptchaTest.Common
{
    public class ReCaptchaResponse
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string apk_package_name { get; set; }
    }
}