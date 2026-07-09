using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Helpers
{
    public static class PlaywrightExtensions
    {
        public static async Task ClickWithOverloadCheckAsync(this IPage page, string selector)
        {
            await page.ClickAsync(selector);
            await page.CheckAndReloadIfOverloadedAsync();
        }

        public static async Task CheckAndReloadIfOverloadedAsync(this IPage page)
        {
            // Wait a brief moment to allow page load/response to begin
            await page.WaitForTimeoutAsync(500);

            int maxRetries = 3;
            for (int i = 1; i <= maxRetries; i++)
            {
                var content = await page.ContentAsync();
                if (content.Contains("heavy load (queue full)") || content.Contains("too many people are accessing this website"))
                {
                    if (i == maxRetries)
                    {
                        throw new System.Exception("The website is under heavy load (queue full) after multiple reload retries.");
                    }
                    await page.WaitForTimeoutAsync(3000 * i);
                    await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
                }
                else
                {
                    break;
                }
            }
        }
    }
}
