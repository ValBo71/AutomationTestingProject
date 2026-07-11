using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class PaymentPage : BasePage
    {
        public PaymentPage(IPage page) : base(page)
        {
        }

        public async Task EnterPaymentDetailsAsync(
            string nameOnCard,
            string cardNumber,
            string cvc,
            string expirationMonth,
            string expirationYear)
        {
            await Locator(PaymentPageSelectors.NameOnCardInput).FillAsync(nameOnCard);
            await Locator(PaymentPageSelectors.CardNumberInput).FillAsync(cardNumber);
            await Locator(PaymentPageSelectors.CvcInput).FillAsync(cvc);
            await Locator(PaymentPageSelectors.ExpiryMonthInput).FillAsync(expirationMonth);
            await Locator(PaymentPageSelectors.ExpiryYearInput).FillAsync(expirationYear);
        }

        public async Task ClickPayAndConfirmOrderAsync()
        {
            await Locator(PaymentPageSelectors.PayButton).ClickAsync();
        }

        public async Task<bool> IsOrderSuccessMessageVisibleAsync()
        {
            return await IsVisibleAfterWaitAsync(CartPageSelectors.OrderSuccessMessage);
        }

        public async Task<string> ClickDownloadInvoiceAsync(string directory)
        {
            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Locator(CartPageSelectors.DownloadInvoiceButton).ClickAsync();
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
            await Locator(CartPageSelectors.OrderContinueButton).ClickAsync();
        }
    }
}