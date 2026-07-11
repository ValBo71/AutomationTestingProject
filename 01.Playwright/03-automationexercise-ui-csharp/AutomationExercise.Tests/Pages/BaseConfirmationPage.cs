using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Pages
{
    public abstract class BaseConfirmationPage : BasePage
    {
        private readonly string _headerSelector;
        private const string ContinueButtonSelector = "a[data-qa='continue-button']";

        protected BaseConfirmationPage(IPage page, string headerSelector) : base(page)
        {
            _headerSelector = headerSelector;
        }

        protected async Task<bool> IsHeaderVisibleAsync()
        {
            return await IsVisibleAfterWaitAsync(_headerSelector);
        }

        public async Task ClickContinueAsync()
        {
            await Locator(ContinueButtonSelector).ClickAsync();
        }
    }
}
