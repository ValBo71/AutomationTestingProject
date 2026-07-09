using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class SignupPage
    {
        private readonly IPage _page;

        public SignupPage(IPage page)
        {
            _page = page;
        }

        // Form Fields Selectors
        public const string GenderMaleRadio = "#id_gender1";
        public const string GenderFemaleRadio = "#id_gender2";
        public const string PasswordInput = "#password";
        public const string DaysSelect = "#days";
        public const string MonthsSelect = "#months";
        public const string YearsSelect = "#years";
        public const string NewsletterCheckbox = "#newsletter";
        public const string OptinCheckbox = "#optin";
        public const string FirstNameInput = "#first_name";
        public const string LastNameInput = "#last_name";
        public const string CompanyInput = "#company";
        public const string Address1Input = "#address1";
        public const string Address2Input = "#address2";
        public const string CountrySelect = "#country";
        public const string StateInput = "#state";
        public const string CityInput = "#city";
        public const string ZipcodeInput = "#zipcode";
        public const string MobileNumberInput = "#mobile_number";
        public const string CreateAccountButton = "button[data-qa='create-account']";

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
            await _page.ClickAsync(GenderMaleRadio);
            await _page.FillAsync(PasswordInput, password);
            await _page.SelectOptionAsync(DaysSelect, new[] { day });
            await _page.SelectOptionAsync(MonthsSelect, new[] { month });
            await _page.SelectOptionAsync(YearsSelect, new[] { year });
            
            await _page.CheckAsync(NewsletterCheckbox);
            await _page.CheckAsync(OptinCheckbox);

            await _page.FillAsync(FirstNameInput, firstName);
            await _page.FillAsync(LastNameInput, lastName);
            
            if (!string.IsNullOrEmpty(company))
            {
                await _page.FillAsync(CompanyInput, company);
            }
            
            await _page.FillAsync(Address1Input, address1);
            
            if (!string.IsNullOrEmpty(address2))
            {
                await _page.FillAsync(Address2Input, address2);
            }
            
            await _page.SelectOptionAsync(CountrySelect, new[] { country });
            await _page.FillAsync(StateInput, state);
            await _page.FillAsync(CityInput, city);
            await _page.FillAsync(ZipcodeInput, zipcode);
            await _page.FillAsync(MobileNumberInput, mobileNumber);
        }

        public async Task ClickCreateAccountAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CreateAccountButton);
        }
    }
}