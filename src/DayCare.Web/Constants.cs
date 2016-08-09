namespace DayCare.Web
{
    public static class Constants
    {
        public const string TempCookieMiddlewareScheme = "TempCookie";
        public const string AppCookieMiddlewareScheme = "AppCookie";
        public const string GitHubMiddlewareScheme = "GitHub";

        public const string DeniedPath = "/Security/Denied";
        public const string LoginPath = "/Security/Login";
        public const string GitHubCallBackPath = "/Security/GitHubCallback";

        public const string GuardianPolicyName = "Guardian";
        public const string StaffPolicyName = "Staff";        
    }
}
