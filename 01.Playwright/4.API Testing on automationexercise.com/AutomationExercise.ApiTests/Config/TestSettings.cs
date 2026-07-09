namespace AutomationExercise.ApiTests.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public DefaultUserCredentials DefaultUser { get; set; } = new();
        public ApiSettings Api { get; set; } = new();
    }

    public class DefaultUserCredentials
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ApiSettings
    {
        public int TimeoutMilliseconds { get; set; } = 30000;
    }
}
