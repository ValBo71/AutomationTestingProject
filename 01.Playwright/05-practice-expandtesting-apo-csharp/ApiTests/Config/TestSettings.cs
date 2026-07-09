namespace ApiTests.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public ApiSettings Api { get; set; } = new();
        public AuthenticationSettings Authentication { get; set; } = new();
    }

    public class ApiSettings
    {
        public int TimeoutMilliseconds { get; set; } = 30000;
    }

    public class AuthenticationSettings
    {
        public string Type { get; set; } = "ApiKey";
        public string HeaderName { get; set; } = "x-auth-token";
    }
}
