namespace AutomationExercise.ApiTests.Config
{
    /// <summary>
    /// No account credentials live here on purpose: every test that needs an account registers a
    /// throwaway one with generated data and deletes it afterwards (see TestUsers).
    /// </summary>
    public class TestSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public ApiSettings Api { get; set; } = new();
    }

    public class ApiSettings
    {
        public int TimeoutMilliseconds { get; set; } = 30000;
    }
}
