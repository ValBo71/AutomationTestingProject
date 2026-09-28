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

        /// <summary>
        /// Waits for the cart to render - either a product row or the "Cart is empty!" notice - before
        /// counting. Counting straight after navigation read an unrendered table as 0, which made a
        /// "cart is empty" check pass for the wrong reason and a "cart has items" check fail for none.
        /// </summary>
        public async Task<int> GetCartItemCountAsync()
        {
            await Locator($"{CartPageSelectors.CartItems}, {CartPageSelectors.EmptyCartContainer}").First
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            if (await Locator(CartPageSelectors.EmptyCartContainer).IsVisibleAsync())
            {
                return 0;
            }
            return await Locator(CartPageSelectors.CartItems).CountAsync();
        }

        /// <summary>
        /// Fails when there is nothing to remove. It used to skip the click on an empty cart, so a test
        /// that then expected an empty cart passed without having removed anything.
        /// </summary>
        public async Task RemoveFirstItemAsync()
        {
            var initialCount = await GetCartItemCountAsync();
            if (initialCount == 0)
            {
                throw new System.InvalidOperationException("The cart is empty, so there is no product to remove.");
            }

            var rows = Locator(CartPageSelectors.CartItems);
            await rows.First.Locator(CartPageSelectors.CartItemRemoveButton).ClickAsync();

            // The row is removed by the page's own script; wait until the table has one row fewer.
            await Assertions.Expect(rows).ToHaveCountAsync(initialCount - 1);
        }

        /// <summary>Name, unit price, quantity and line total of every row, as the cart shows them.</summary>
        public async Task<System.Collections.Generic.List<(string Name, int Price, int Quantity, int Total)>> GetCartLinesAsync()
        {
            await GetCartItemCountAsync();
            var lines = new System.Collections.Generic.List<(string, int, int, int)>();
            var rows = Locator(CartPageSelectors.CartItems);
            for (var i = 0; i < await rows.CountAsync(); i++)
            {
                var row = rows.Nth(i);
                lines.Add((
                    (await row.Locator(CartPageSelectors.CartItemName).InnerTextAsync()).Trim(),
                    ParseRupees(await row.Locator(CartPageSelectors.CartItemPrice).InnerTextAsync()),
                    int.Parse((await row.Locator(CartPageSelectors.CartItemQuantity).InnerTextAsync()).Trim()),
                    ParseRupees(await row.Locator(CartPageSelectors.CartItemTotalPrice).InnerTextAsync())));
            }
            return lines;
        }

        /// <summary>"Rs. 500" -> 500.</summary>
        private static int ParseRupees(string text) => int.Parse(text.Replace("Rs.", string.Empty).Trim());

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
