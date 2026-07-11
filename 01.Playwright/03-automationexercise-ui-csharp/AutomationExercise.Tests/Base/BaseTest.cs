using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Threading.Tasks;
using AutomationExercise.Tests.Drivers;
using AutomationExercise.Tests.Helpers;
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

            await Context.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            var testFailed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
            var testName = TestContext.CurrentContext.Test.Name;

            if (testFailed)
            {
                try
                {
                    var screenshotPath = await ScreenshotHelper.TakeScreenshotAsync(Page, testName);
                    if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                    {
                        AllureHelper.AddScreenshotAttachment("Failure Screenshot", screenshotPath);
                    }
                }
                catch (System.Exception ex)
                {
                    Infrastructure.TestLog.Warn($"Failed to take failure screenshot: {ex.Message}");
                }

                try
                {
                    var tracesDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "traces");
                    Directory.CreateDirectory(tracesDir);
                    var tracePath = Path.Combine(tracesDir, $"{testName}.zip");
                    await Context.Tracing.StopAsync(new()
                    {
                        Path = tracePath
                    });

                    if (File.Exists(tracePath))
                    {
                        AllureHelper.AddTraceAttachment("Failure Playwright Trace", tracePath);
                    }
                }
                catch (System.Exception ex)
                {
                    Infrastructure.TestLog.Warn($"Failed to save Playwright trace: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    await Context.Tracing.StopAsync();
                }
                catch
                {
                    // Ignore trace stop failures on success
                }
            }

            // Harden teardown against leaks
            try
            {
                if (Context != null)
                {
                    await Context.CloseAsync();
                }
            }
            catch (System.Exception ex)
            {
                Infrastructure.TestLog.Warn($"Error closing browser context: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (Browser != null)
                    {
                        await Browser.CloseAsync();
                    }
                }
                catch (System.Exception ex)
                {
                    Infrastructure.TestLog.Warn($"Error closing browser: {ex.Message}");
                }
                finally
                {
                    try
                    {
                        Playwright?.Dispose();
                    }
                    catch (System.Exception ex)
                    {
                        Infrastructure.TestLog.Warn($"Error disposing Playwright: {ex.Message}");
                    }
                }
            }
        }
    }
}