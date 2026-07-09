using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class CheckoutPage
    {
        private readonly IPage _page;

        public CheckoutPage(IPage page)
        {
            _page = page;
        }

        public async Task<string> GetDeliveryAddressTextAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(CartPageSelectors.AddressDeliveryList);
            return await _page.InnerTextAsync(CartPageSelectors.AddressDeliveryList);
        }

        public async Task<string> GetBillingAddressTextAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(CartPageSelectors.AddressInvoiceList);
            return await _page.InnerTextAsync(CartPageSelectors.AddressInvoiceList);
        }

        public async Task EnterCommentAsync(string comment)
        {
            await _page.FillAsync(CartPageSelectors.CheckoutCommentInput, comment);
        }

        public async Task ClickPlaceOrderAsync()
        {
            await _page.ClickWithOverloadCheckAsync(CartPageSelectors.PlaceOrderButton);
        }
    }
}
