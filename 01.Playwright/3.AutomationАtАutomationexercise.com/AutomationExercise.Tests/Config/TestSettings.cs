namespace AutomationExercise.Tests.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = "https://automationexercise.com";
        public string Browser { get; set; } = "Chromium";
        public bool Headless { get; set; } = false;
        public int TimeoutSeconds { get; set; } = 30;
        public int SlowMoMs { get; set; } = 0;
    }
}
