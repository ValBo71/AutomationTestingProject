using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Threading.Tasks;
using AutomationExercise.Tests.Drivers;
using AutomationExercise.Tests.Helpers;
using Allure.Net.Commons;
using Allure.NUnit;
using System.IO;

namespace AutomationExercise.Tests.Base
{
    public class BaseTest
    {
        protected IPlaywright Playwright { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext Context { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;

        [SetUp]
        public async Task Setup()
        {
            var driver = await PlaywrightDriver.CreateDriverAsync();
            Playwright = driver.playwright;
            Browser = driver.browser;
            Context = driver.context;
            Page = driver.page;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var testName = TestContext.CurrentContext.Test.Name;
                var screenshotPath = await ScreenshotHelper.TakeScreenshotAsync(Page, testName);

                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    AllureHelper.AddScreenshotAttachment("Failure Screenshot", screenshotPath);
                }
            }

            await Context.CloseAsync();
            await Browser.CloseAsync();
            Playwright.Dispose();
        }
    }
}