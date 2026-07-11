using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Pages
{
    public class SignupPage : BasePage
    {
        public SignupPage(IPage page) : base(page)
        {
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
            await Locator(GenderMaleRadio).ClickAsync();
            await Locator(PasswordInput).FillAsync(password);
            await Locator(DaysSelect).SelectOptionAsync(day);
            await Locator(MonthsSelect).SelectOptionAsync(month);
            await Locator(YearsSelect).SelectOptionAsync(year);
            
            await Locator(NewsletterCheckbox).CheckAsync();
            await Locator(OptinCheckbox).CheckAsync();

            await Locator(FirstNameInput).FillAsync(firstName);
            await Locator(LastNameInput).FillAsync(lastName);
            
            if (!string.IsNullOrEmpty(company))
            {
                await Locator(CompanyInput).FillAsync(company);
            }
            
            await Locator(Address1Input).FillAsync(address1);
            
            if (!string.IsNullOrEmpty(address2))
            {
                await Locator(Address2Input).FillAsync(address2);
            }
            
            await Locator(CountrySelect).SelectOptionAsync(country);
            await Locator(StateInput).FillAsync(state);
            await Locator(CityInput).FillAsync(city);
            await Locator(ZipcodeInput).FillAsync(zipcode);
            await Locator(MobileNumberInput).FillAsync(mobileNumber);
        }

        public async Task ClickCreateAccountAsync()
        {
            await Locator(CreateAccountButton).ClickAsync();
        }
    }
}