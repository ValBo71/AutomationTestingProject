using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class CheckoutPage : BasePage
    {
        public CheckoutPage(IPage page) : base(page)
        {
        }

        public async Task<string> GetDeliveryAddressTextAsync()
        {
            var deliveryList = Locator(CartPageSelectors.AddressDeliveryList);
            await deliveryList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await deliveryList.InnerTextAsync();
        }

        public async Task<string> GetBillingAddressTextAsync()
        {
            var invoiceList = Locator(CartPageSelectors.AddressInvoiceList);
            await invoiceList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await invoiceList.InnerTextAsync();
        }

        /// <summary>Product names in the "Review Your Order" table.</summary>
        public async Task<System.Collections.Generic.IReadOnlyList<string>> GetReviewedProductNamesAsync()
        {
            var names = Locator($"{CartPageSelectors.CartItems} {CartPageSelectors.CartItemName}");
            await names.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var texts = await names.AllInnerTextsAsync();
            return System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(texts, t => t.Trim()));
        }

        public async Task EnterCommentAsync(string comment)
        {
            await Locator(CartPageSelectors.CheckoutCommentInput).FillAsync(comment);
        }

        public async Task ClickPlaceOrderAsync()
        {
            await Locator(CartPageSelectors.PlaceOrderButton).ClickAsync();
        }
    }
}
