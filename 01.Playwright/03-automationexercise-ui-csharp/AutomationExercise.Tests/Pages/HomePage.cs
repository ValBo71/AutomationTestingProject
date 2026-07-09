using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Drivers;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class HomePage
    {
        private readonly IPage _page;

        public HomePage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            int maxRetries = 3;
            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    await _page.GotoAsync(PlaywrightDriver.Settings.BaseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
                    var content = await _page.ContentAsync();
                    if (content.Contains("heavy load (queue full)") || content.Contains("too many people are accessing this website"))
                    {
                        if (i == maxRetries)
                        {
                            throw new System.Exception("The website is under heavy load (queue full) after multiple retries.");
                        }
                        await _page.WaitForTimeoutAsync(3000 * i);
                        continue;
                    }
                    break;
                }
                catch (System.Exception) when (i < maxRetries)
                {
                    await _page.WaitForTimeoutAsync(3000 * i);
                }
            }
            await HandleConsentDialogAsync();
        }

        public async Task HandleConsentDialogAsync()
        {
            var consentButton = _page.Locator(".fc-consent-root button.fc-cta-consent");
            try
            {
                await consentButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 3000 });
                await consentButton.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 2000 });
                await _page.WaitForTimeoutAsync(500);
            }
            catch (System.Exception)
            {
                // Dialog did not appear, ignore
            }
        }

        public async Task ClickProductsAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationProducts);
        }

        public async Task ClickCartAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationCart);
        }

        public async Task ClickLoginSignupAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationSignupLogin);
        }

        public async Task ClickContactUsAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationContactUs);
        }

        public async Task ClickTestCasesAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationTestCases);
        }

        public async Task ClickLogoutAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationLogout);
        }

        public async Task ClickDeleteAccountAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CommonSelectors.NavigationDeleteAccount);
        }

        public async Task<bool> IsLoggedInUserVisibleAsync(string username)
        {
            await _page.WaitForSelectorAsync(CommonSelectors.LoggedInUserText);
            var text = await _page.InnerTextAsync(CommonSelectors.LoggedInUserText);
            return text.Contains(username);
        }

        public async Task SubscribeAsync(string email)
        {
            await _page.Locator(HomePageSelectors.SubscriptionEmailInput).ScrollIntoViewIfNeededAsync();
            await _page.FillAsync(HomePageSelectors.SubscriptionEmailInput, email);
            await _page.ClickAsync(HomePageSelectors.SubscriptionButton);
        }

        public async Task<bool> IsSubscriptionSuccessVisibleAsync()
        {
            await _page.WaitForSelectorAsync(HomePageSelectors.SubscriptionSuccessMessage);
            return await _page.IsVisibleAsync(HomePageSelectors.SubscriptionSuccessMessage);
        }

        public async Task AddFirstRecommendedItemToCartAsync()
        {
            await _page.Locator(HomePageSelectors.RecommendedItemsHeader).First.ScrollIntoViewIfNeededAsync();
            await _page.ClickAsync(HomePageSelectors.FirstRecommendedItemAddToCart);
            // Click continue shopping or modal view cart
            await _page.ClickAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }
    }
}