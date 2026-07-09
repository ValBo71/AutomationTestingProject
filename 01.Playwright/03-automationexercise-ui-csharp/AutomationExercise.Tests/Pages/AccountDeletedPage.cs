using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class AccountDeletedPage
    {
        private readonly IPage _page;

        public AccountDeletedPage(IPage page)
        {
            _page = page;
        }

        private const string AccountDeletedHeader = "h2[data-qa='account-deleted']";
        private const string ContinueButton = "a[data-qa='continue-button']";

        public async Task<bool> IsAccountDeletedVisibleAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(AccountDeletedHeader);
            return await _page.IsVisibleAsync(AccountDeletedHeader);
        }

        public async Task ClickContinueAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.ClickWithOverloadCheckAsync(ContinueButton);
        }
    }
}
