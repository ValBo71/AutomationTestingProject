using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page)
        {
            Page = page;
        }

        protected ILocator Locator(string selector)
        {
            return Page.Locator(selector);
        }

        /// <summary>
        /// Waits for the selector to become visible, then reports its visibility.
        /// Shared helper for the common "wait for it, then check it's shown" page-object pattern.
        /// </summary>
        protected async Task<bool> IsVisibleAfterWaitAsync(string selector)
        {
            var element = Locator(selector);
            await element.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await element.IsVisibleAsync();
        }
    }
}
