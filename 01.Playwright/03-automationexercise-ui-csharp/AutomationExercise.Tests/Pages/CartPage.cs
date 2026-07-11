using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

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
            var selector = $"tr[id^='product-']:nth-child({rowIndex}) {CartPageSelectors.CartItemQuantity}";
            var locator = Locator(selector);
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var qtyText = await locator.InnerTextAsync();
            return int.Parse(qtyText);
        }

        public async Task ClickProceedToCheckoutAsync()
        {
            await Locator(CartPageSelectors.ProceedToCheckoutButton).ClickAsync();
        }

        public async Task<bool> IsEmptyCartMessageVisibleAsync()
        {
            var emptyCartMsg = Locator(CartPageSelectors.EmptyCartContainer);
            await emptyCartMsg.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await emptyCartMsg.IsVisibleAsync();
        }
    }
}
