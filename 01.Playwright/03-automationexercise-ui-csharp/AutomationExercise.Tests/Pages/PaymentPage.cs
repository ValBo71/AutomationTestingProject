using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class PaymentPage
    {
        private readonly IPage _page;

        public PaymentPage(IPage page)
        {
            _page = page;
        }

        // Selectors
        public const string NameOnCardInput = "input[data-qa='name-on-card']";
        public const string CardNumberInput = "input[data-qa='card-number']";
        public const string CvcInput = "input[data-qa='cvc']";
        public const string ExpiryMonthInput = "input[data-qa='expiry-month']";
        public const string ExpiryYearInput = "input[data-qa='expiry-year']";
        public const string PayButton = "button[data-qa='pay-button']";

        public async Task EnterPaymentDetailsAsync(
            string nameOnCard,
            string cardNumber,
            string cvc,
            string expirationMonth,
            string expirationYear)
        {
            await _page.FillAsync(NameOnCardInput, nameOnCard);
            await _page.FillAsync(CardNumberInput, cardNumber);
            await _page.FillAsync(CvcInput, cvc);
            await _page.FillAsync(ExpiryMonthInput, expirationMonth);
            await _page.FillAsync(ExpiryYearInput, expirationYear);
        }

        public async Task ClickPayAndConfirmOrderAsync()
        {
            await _page.ClickWithOverloadCheckAsync(PayButton);
        }

        public async Task<bool> IsOrderSuccessMessageVisibleAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(CartPageSelectors.OrderSuccessMessage);
            return await _page.IsVisibleAsync(CartPageSelectors.OrderSuccessMessage);
        }

        public async Task<string> ClickDownloadInvoiceAsync(string directory)
        {
            var download = await _page.RunAndWaitForDownloadAsync(async () =>
            {
                await _page.ClickAsync(CartPageSelectors.DownloadInvoiceButton);
            });

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            var filePath = System.IO.Path.Combine(directory, download.SuggestedFilename);
            await download.SaveAsAsync(filePath);
            return filePath;
        }

        public async Task ClickContinueAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.ClickWithOverloadCheckAsync(CartPageSelectors.OrderContinueButton);
        }
    }
}