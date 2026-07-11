using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class SignupPage : BasePage
    {
        public SignupPage(IPage page) : base(page)
        {
        }

        public async Task FillSignupDetailsAsync(
            string password,
            string day,
            string month,
            string year,
            string firstName,
            string lastName,
            string address1,
            string country,
            string state,
            string city,
            string zipcode,
            string mobileNumber,
            string company = "",
            string address2 = "")
        {
            await Locator(SignupPageSelectors.GenderMaleRadio).ClickAsync();
            await Locator(SignupPageSelectors.PasswordInput).FillAsync(password);
            await Locator(SignupPageSelectors.DaysSelect).SelectOptionAsync(day);
            await Locator(SignupPageSelectors.MonthsSelect).SelectOptionAsync(month);
            await Locator(SignupPageSelectors.YearsSelect).SelectOptionAsync(year);

            await Locator(SignupPageSelectors.NewsletterCheckbox).CheckAsync();
            await Locator(SignupPageSelectors.OptinCheckbox).CheckAsync();

            await Locator(SignupPageSelectors.FirstNameInput).FillAsync(firstName);
            await Locator(SignupPageSelectors.LastNameInput).FillAsync(lastName);

            if (!string.IsNullOrEmpty(company))
            {
                await Locator(SignupPageSelectors.CompanyInput).FillAsync(company);
            }

            await Locator(SignupPageSelectors.Address1Input).FillAsync(address1);

            if (!string.IsNullOrEmpty(address2))
            {
                await Locator(SignupPageSelectors.Address2Input).FillAsync(address2);
            }

            await Locator(SignupPageSelectors.CountrySelect).SelectOptionAsync(country);
            await Locator(SignupPageSelectors.StateInput).FillAsync(state);
            await Locator(SignupPageSelectors.CityInput).FillAsync(city);
            await Locator(SignupPageSelectors.ZipcodeInput).FillAsync(zipcode);
            await Locator(SignupPageSelectors.MobileNumberInput).FillAsync(mobileNumber);
        }

        public async Task ClickCreateAccountAsync()
        {
            await Locator(SignupPageSelectors.CreateAccountButton).ClickAsync();
        }
    }
}
