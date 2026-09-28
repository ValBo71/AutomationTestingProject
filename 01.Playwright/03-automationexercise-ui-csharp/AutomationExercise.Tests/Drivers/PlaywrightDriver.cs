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
            
            // Allow overriding headless mode and slow motion via environment variables
            string? headlessEnv = Environment.GetEnvironmentVariable("HEADLESS") 
                                  ?? Environment.GetEnvironmentVariable("TestSettings__Headless");
            bool headless = !string.IsNullOrEmpty(headlessEnv)
                ? bool.Parse(headlessEnv)
                : Settings.Headless;

            string? slowMoEnv = Environment.GetEnvironmentVariable("SLOW_MO_MS") 
                                ?? Environment.GetEnvironmentVariable("TestSettings__SlowMoMs");
            int slowMoMs = !string.IsNullOrEmpty(slowMoEnv)
                ? int.Parse(slowMoEnv)
                : Settings.SlowMoMs;

            BrowserTypeLaunchOptions launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMoMs > 0 ? slowMoMs : null
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
            context.SetDefaultTimeout(Settings.TimeoutSeconds * 1000);

            // Block ads, consent overlays and trackers by host. Matching the host matters: the earlier
            // glob patterns such as "**/*google*" never matched these requests, because "*" in a Playwright
            // glob does not cross a "/", so only the last path segment was tested. The ad scripts
            // (pagead2.googlesyndication.com) and the "fundingchoices" vignette overlay that covers the
            // page therefore still loaded, and were a large part of why these tests needed retries.
            await context.RouteAsync(url => IsBlockedHost(url), route => route.AbortAsync());

            var page = await context.NewPageAsync();
            return (playwright, browser, context, page);
        }

        // Covers googlesyndication, fundingchoicesmessages.google.com, googletagmanager, fonts.googleapis
        // and the other trackers the site loads; the site itself (automationexercise.com) matches none.
        private static readonly string[] BlockedHostFragments =
            { "google", "doubleclick", "facebook", "quantserve", "adservice", "analytics" };

        private static bool IsBlockedHost(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            foreach (var fragment in BlockedHostFragments)
            {
                if (uri.Host.Contains(fragment, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }
}