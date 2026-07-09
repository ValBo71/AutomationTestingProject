using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        public async Task LoginAsync(string email, string password)
        {
            await _page.FillAsync(LoginPageSelectors.LoginEmailInput, email);
            await _page.FillAsync(LoginPageSelectors.LoginPasswordInput, password);
            await _page.ClickWithOverloadCheckAsync(LoginPageSelectors.LoginButton);
        }

        public async Task SignUpInitAsync(string name, string email)
        {
            await _page.FillAsync(LoginPageSelectors.SignupNameInput, name);
            await _page.FillAsync(LoginPageSelectors.SignupEmailInput, email);
            await _page.ClickWithOverloadCheckAsync(LoginPageSelectors.SignupButton);
        }

        public async Task<string> GetLoginErrorTextAsync()
        {
            await _page.WaitForSelectorAsync(LoginPageSelectors.LoginErrorMessage);
            return await _page.InnerTextAsync(LoginPageSelectors.LoginErrorMessage);
        }

        public async Task<string> GetSignupErrorTextAsync()
        {
            await _page.WaitForSelectorAsync(LoginPageSelectors.SignupErrorMessage);
            return await _page.InnerTextAsync(LoginPageSelectors.SignupErrorMessage);
        }
        
        public async Task<bool> IsSignupFormVisibleAsync()
        {
            return await _page.IsVisibleAsync(LoginPageSelectors.SignupNameInput);
        }
    }
}
