using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task LoginAsync(string email, string password)
        {
            await Locator(LoginPageSelectors.LoginEmailInput).FillAsync(email);
            await Locator(LoginPageSelectors.LoginPasswordInput).FillAsync(password);
            await Locator(LoginPageSelectors.LoginButton).ClickAsync();
        }

        public async Task SignUpInitAsync(string name, string email)
        {
            await Locator(LoginPageSelectors.SignupNameInput).FillAsync(name);
            await Locator(LoginPageSelectors.SignupEmailInput).FillAsync(email);
            await Locator(LoginPageSelectors.SignupButton).ClickAsync();
        }

        public async Task<string> GetLoginErrorTextAsync()
        {
            var errorMsg = Locator(LoginPageSelectors.LoginErrorMessage);
            await errorMsg.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await errorMsg.InnerTextAsync();
        }

        public async Task<string> GetSignupErrorTextAsync()
        {
            var errorMsg = Locator(LoginPageSelectors.SignupErrorMessage);
            await errorMsg.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await errorMsg.InnerTextAsync();
        }
        
        public async Task<bool> IsSignupFormVisibleAsync()
        {
            return await Locator(LoginPageSelectors.SignupNameInput).IsVisibleAsync();
        }
    }
}
