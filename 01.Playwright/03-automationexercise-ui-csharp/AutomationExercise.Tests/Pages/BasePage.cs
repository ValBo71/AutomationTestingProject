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
    }
}
