using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Pages
{
    public class AccountDeletedPage : BaseConfirmationPage
    {
        private const string AccountDeletedHeader = "h2[data-qa='account-deleted']";

        public AccountDeletedPage(IPage page) : base(page, AccountDeletedHeader)
        {
        }

        public Task<bool> IsAccountDeletedVisibleAsync() => IsHeaderVisibleAsync();
    }
}
