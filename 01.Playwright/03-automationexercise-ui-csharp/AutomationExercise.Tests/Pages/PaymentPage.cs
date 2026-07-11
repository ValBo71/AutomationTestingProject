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
            await Locator(NameOnCardInput).FillAsync(nameOnCard);
            await Locator(CardNumberInput).FillAsync(cardNumber);
            await Locator(CvcInput).FillAsync(cvc);
            await Locator(ExpiryMonthInput).FillAsync(expirationMonth);
            await Locator(ExpiryYearInput).FillAsync(expirationYear);
        }

        public async Task ClickPayAndConfirmOrderAsync()
        {
            await Locator(PayButton).ClickAsync();
        }

        public async Task<bool> IsOrderSuccessMessageVisibleAsync()
        {
            var successMsg = Locator(CartPageSelectors.OrderSuccessMessage);
            await successMsg.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await successMsg.IsVisibleAsync();
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