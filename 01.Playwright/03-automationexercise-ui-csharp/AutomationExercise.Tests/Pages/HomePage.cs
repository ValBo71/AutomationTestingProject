using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Drivers;
using AutomationExercise.Tests.Infrastructure;
using System;

namespace AutomationExercise.Tests.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IPage page) : base(page)
        {
        }

        public async Task NavigateAsync()
        {
            await SiteAvailability.GotoWithRetryAsync(Page, PlaywrightDriver.Settings.BaseUrl);
            await HandleConsentDialogAsync();
        }

        public async Task HandleConsentDialogAsync()
        {
            var consentButton = Locator(".fc-consent-root button.fc-cta-consent");
            try
            {
                await consentButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 3000 });
                await consentButton.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 2000 });
            }
            catch (TimeoutException)
            {
                TestLog.Debug("Consent dialog did not appear within timeout. Continuing...");
            }
            catch (PlaywrightException ex)
            {
                TestLog.Warn($"Playwright error while handling the consent dialog: {ex.Message}");
            }
        }

        public async Task ClickProductsAsync()
        {
            await Locator(CommonSelectors.NavigationProducts).ClickAsync();
        }

        public async Task ClickCartAsync()
        {
            await Locator(CommonSelectors.NavigationCart).ClickAsync();
        }

        public async Task ClickLoginSignupAsync()
        {
            await Locator(CommonSelectors.NavigationSignupLogin).ClickAsync();
        }

        public async Task ClickContactUsAsync()
        {
            await Locator(CommonSelectors.NavigationContactUs).ClickAsync();
        }

        public async Task ClickTestCasesAsync()
        {
            await Locator(CommonSelectors.NavigationTestCases).ClickAsync();
        }

        public async Task ClickLogoutAsync()
        {
            await Locator(CommonSelectors.NavigationLogout).ClickAsync();
        }

        public async Task ClickDeleteAccountAsync()
        {
            await Locator(CommonSelectors.NavigationDeleteAccount).ClickAsync();
        }

        public async Task<bool> IsLoggedInUserVisibleAsync(string username)
        {
            try
            {
                await Assertions.Expect(Locator(CommonSelectors.LoggedInUserText)).ToContainTextAsync(username);
                return true;
            }
            catch (PlaywrightException ex) when (!ex.Message.Contains("closed", System.StringComparison.OrdinalIgnoreCase))
            {
                // Assertion timed out because the element genuinely never appeared - not a crash/closed page.
                return false;
            }
        }

        public async Task SubscribeAsync(string email)
        {
            var emailInput = Locator(HomePageSelectors.SubscriptionEmailInput);
            await emailInput.ScrollIntoViewIfNeededAsync();
            await emailInput.FillAsync(email);
            await Locator(HomePageSelectors.SubscriptionButton).ClickAsync();
        }

        public async Task<bool> IsSubscriptionSuccessVisibleAsync()
        {
            try
            {
                await Assertions.Expect(Locator(HomePageSelectors.SubscriptionSuccessMessage)).ToBeVisibleAsync();
                return true;
            }
            catch (PlaywrightException ex) when (!ex.Message.Contains("closed", System.StringComparison.OrdinalIgnoreCase))
            {
                // Assertion timed out because the element genuinely never appeared - not a crash/closed page.
                return false;
            }
        }

        public async Task AddFirstRecommendedItemToCartAsync()
        {
            await Locator(HomePageSelectors.RecommendedItemsHeader).First.ScrollIntoViewIfNeededAsync();
            await Locator(HomePageSelectors.FirstRecommendedItemAddToCart).ClickAsync();
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).ClickAsync();
        }
    }
}