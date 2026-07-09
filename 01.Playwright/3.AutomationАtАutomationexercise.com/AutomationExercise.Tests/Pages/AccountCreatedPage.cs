using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class AccountCreatedPage
    {
        private readonly IPage _page;

        public AccountCreatedPage(IPage page)
        {
            _page = page;
        }

        private const string AccountCreatedHeader = "h2[data-qa='account-created']";
        private const string ContinueButton = "a[data-qa='continue-button']";

        public async Task<bool> IsAccountCreatedVisibleAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(AccountCreatedHeader);
            return await _page.IsVisibleAsync(AccountCreatedHeader);
        }

        public async Task ClickContinueAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.ClickWithOverloadCheckAsync(ContinueButton);
        }
    }
}
