using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using AutomationExercise.Tests.Infrastructure;

namespace AutomationExercise.Tests.Helpers
{
    public static class ScreenshotHelper
    {
        public static async Task<string> TakeScreenshotAsync(IPage page, string testName)
        {
            try
            {
                var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestResults", "Screenshots");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                // Remove invalid filename characters
                var cleanTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
                var filename = $"{cleanTestName}_{timestamp}.png";
                var fullPath = Path.Combine(directory, filename);

                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = fullPath,
                    FullPage = true
                });

                return fullPath;
            }
            catch (Exception ex)
            {
                TestLog.Warn($"Failed to take screenshot: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
