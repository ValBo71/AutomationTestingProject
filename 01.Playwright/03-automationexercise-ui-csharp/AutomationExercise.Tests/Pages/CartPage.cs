using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Infrastructure;

namespace AutomationExercise.Tests.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IPage page) : base(page)
        {
        }

        public async Task<int> GetCartItemCountAsync()
        {
            if (await Locator(CartPageSelectors.EmptyCartContainer).IsVisibleAsync())
            {
                return 0;
            }
            return await Locator(CartPageSelectors.CartItems).CountAsync();
        }

        public async Task RemoveFirstItemAsync()
        {
            var initialCount = await GetCartItemCountAsync();
            if (initialCount > 0)
            {
                await Locator($"{CartPageSelectors.CartItems}:first-child {CartPageSelectors.CartItemRemoveButton}").ClickAsync();
                
                // Web-first wait: wait for the first cart item row to be detached/removed
                var firstItem = Locator(CartPageSelectors.CartItems).First;
                await firstItem.WaitForAsync(new() { State = WaitForSelectorState.Detached });
            }
        }

        public async Task<int> GetCartItemQuantityAsync(int rowIndex)
        {
            var selector = $"{CartPageSelectors.CartItems}:nth-child({rowIndex}) {CartPageSelectors.CartItemQuantity}";
            var locator = Locator(selector);
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var qtyText = await locator.InnerTextAsync();
            return int.Parse(qtyText);
        }

        public async Task ClickProceedToCheckoutAsync()
        {
            await Locator(CartPageSelectors.ProceedToCheckoutButton).ClickAsync();
        }

        /// <summary>
        /// Clicks the "Register / Login" link inside the checkout modal that appears after
        /// Proceed to Checkout for a guest. If the modal is slow to appear, retries once by
        /// clicking Proceed to Checkout again instead of failing immediately.
        /// </summary>
        public async Task ClickRegisterLoginInCheckoutModalAsync()
        {
            var registerLoginLocator = Locator(CartPageSelectors.RegisterLoginModalLink);
            try
            {
                await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
            }
            catch (PlaywrightException ex)
            {
                TestLog.Warn($"Register/Login link did not appear: {ex.Message}. Retrying proceed to checkout...");
                await ClickProceedToCheckoutAsync();
                await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
            }
            await registerLoginLocator.ClickAsync();
        }

        public async Task<bool> IsEmptyCartMessageVisibleAsync()
        {
            return await IsVisibleAfterWaitAsync(CartPageSelectors.EmptyCartContainer);
        }
    }
}
