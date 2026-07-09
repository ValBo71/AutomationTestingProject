using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AutomationExercise.Tests.Drivers
{
    public class PlaywrightDriver
    {
        private static TestSettings? _settings;

        public static TestSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    _settings = new TestSettings();
                    config.GetSection("TestSettings").Bind(_settings);
                }
                return _settings;
            }
        }

        public static async Task<(IPlaywright playwright, IBrowser browser, IBrowserContext context, IPage page)> CreateDriverAsync()
        {
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            
            BrowserTypeLaunchOptions launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = Settings.Headless,
                SlowMo = Settings.SlowMoMs > 0 ? Settings.SlowMoMs : null
            };

            IBrowser browser;
            switch (Settings.Browser.ToLower())
            {
                case "firefox":
                    browser = await playwright.Firefox.LaunchAsync(launchOptions);
                    break;
                case "webkit":
                    browser = await playwright.Webkit.LaunchAsync(launchOptions);
                    break;
                case "chromium":
                default:
                    browser = await playwright.Chromium.LaunchAsync(launchOptions);
                    break;
            }

            var context = await browser.NewContextAsync();

            // Block all Google domains (ads, tag manager, analytics, fonts, etc.) and other trackers
            await context.RouteAsync("**/*google*", route => route.AbortAsync());
            await context.RouteAsync("**/*analytics*", route => route.AbortAsync());
            await context.RouteAsync("**/*doubleclick*", route => route.AbortAsync());
            await context.RouteAsync("**/*pagead*", route => route.AbortAsync());
            await context.RouteAsync("**/*adsbygoogle*", route => route.AbortAsync());
            await context.RouteAsync("**/*facebook*", route => route.AbortAsync());
            await context.RouteAsync("**/*quantserve*", route => route.AbortAsync());
            await context.RouteAsync("**/*adservice*", route => route.AbortAsync());

            var page = await context.NewPageAsync();
            return (playwright, browser, context, page);
        }
    }
}