using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Infrastructure
{
    public static class SiteAvailability
    {
        public static async Task GotoWithRetryAsync(IPage page, string url, int maxAttempts = 3, int baseDelayMs = 3000)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    TestLog.Debug($"Navigating to {url} (Attempt {attempt}/{maxAttempts})");
                    await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
                    
                    var bodyText = await page.Locator("body").InnerTextAsync();
                    
                    if (bodyText.Contains("heavy load (queue full)", StringComparison.OrdinalIgnoreCase) || 
                        bodyText.Contains("too many people are accessing this website", StringComparison.OrdinalIgnoreCase))
                    {
                        if (attempt == maxAttempts)
                        {
                            throw new InvalidOperationException($"The website is under heavy load (queue full) after {maxAttempts} attempts.");
                        }
                        
                        int delay = baseDelayMs * attempt;
                        TestLog.Warn($"Website overloaded. Retrying in {delay}ms...");
                        await page.WaitForTimeoutAsync(delay);
                        continue;
                    }
                    
                    return;
                }
                catch (System.Exception ex) when (attempt < maxAttempts)
                {
                    int delay = baseDelayMs * attempt;
                    TestLog.Warn($"Navigation error: {ex.Message}. Retrying in {delay}ms...");
                    await page.WaitForTimeoutAsync(delay);
                }
            }
            throw new InvalidOperationException($"Failed to navigate to {url} after {maxAttempts} attempts.");
        }
    }
}
