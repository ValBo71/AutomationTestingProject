using System;
using Microsoft.Extensions.Configuration;

namespace AutomationExercise.RestSharp.ApiTests.Config
{
    public static class TestSettings
    {
        private static readonly IConfiguration Configuration;

        static TestSettings()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string BaseUrl => Configuration["BaseUrl"] ?? "https://automationexercise.com";

        public static int TimeoutMilliseconds => int.TryParse(Configuration["Api:TimeoutMilliseconds"], out var timeout) ? timeout : 30000;
    }
}
