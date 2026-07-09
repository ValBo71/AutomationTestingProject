using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class CartPage
    {
        private readonly IPage _page;

        public CartPage(IPage page)
        {
            _page = page;
        }

        public async Task<int> GetCartItemCountAsync()
        {
            // If empty message is shown, count is 0
            if (await _page.IsVisibleAsync(CartPageSelectors.EmptyCartContainer))
            {
                return 0;
            }
            var items = await _page.QuerySelectorAllAsync(CartPageSelectors.CartItems);
            return items.Count;
        }

        public async Task RemoveFirstItemAsync()
        {
            var initialCount = await GetCartItemCountAsync();
            if (initialCount > 0)
            {
                await _page.ClickAsync($"{CartPageSelectors.CartItems}:first-child {CartPageSelectors.CartItemRemoveButton}");
                // Wait for the row to be detached
                await _page.WaitForTimeoutAsync(1000);
            }
        }

        public async Task<int> GetCartItemQuantityAsync(int rowIndex)
        {
            var selector = $"tr[id^='product-']:nth-child({rowIndex}) {CartPageSelectors.CartItemQuantity}";
            await _page.WaitForSelectorAsync(selector);
            var qtyText = await _page.InnerTextAsync(selector);
            return int.Parse(qtyText);
        }

        public async Task ClickProceedToCheckoutAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CartPageSelectors.ProceedToCheckoutButton);
        }

        public async Task<bool> IsEmptyCartMessageVisibleAsync()
        {
            await _page.WaitForSelectorAsync(CartPageSelectors.EmptyCartContainer);
            return await _page.IsVisibleAsync(CartPageSelectors.EmptyCartContainer);
        }
    }
}
