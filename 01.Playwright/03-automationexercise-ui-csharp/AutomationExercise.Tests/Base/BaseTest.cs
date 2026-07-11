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

        /// <summary>
        /// Set to true right after a test registers/logs into an account on the live site, and back to
        /// false once the test's own "delete account" cleanup step succeeds. If a test fails or throws
        /// in between, TearDown uses this flag to attempt a best-effort deletion so failed runs don't
        /// leak accounts on the shared public demo site.
        /// </summary>
        protected bool AccountPendingCleanup { get; set; }

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

            if (AccountPendingCleanup)
            {
                try
                {
                    var homePage = new Pages.HomePage(Page);
                    await homePage.NavigateAsync();
                    await homePage.ClickDeleteAccountAsync();
                    Infrastructure.TestLog.Warn("TearDown safety-net: deleted a leftover account after the test did not reach its own cleanup step.");
                }
                catch (System.Exception ex)
                {
                    Infrastructure.TestLog.Warn($"TearDown safety-net account cleanup failed (account may already be gone or session lost): {ex.Message}");
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