using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Pages
{
    public class AccountCreatedPage : BaseConfirmationPage
    {
        private const string AccountCreatedHeader = "h2[data-qa='account-created']";

        public AccountCreatedPage(IPage page) : base(page, AccountCreatedHeader)
        {
        }

        public Task<bool> IsAccountCreatedVisibleAsync() => IsHeaderVisibleAsync();
    }
}
