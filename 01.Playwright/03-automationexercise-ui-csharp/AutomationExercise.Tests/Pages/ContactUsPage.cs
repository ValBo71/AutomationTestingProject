using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Infrastructure;

namespace AutomationExercise.Tests.Pages
{
    public class ContactUsPage : BasePage
    {
        public ContactUsPage(IPage page) : base(page)
        {
        }

        public async Task SubmitContactFormAsync(
            string name,
            string email,
            string subject,
            string message,
            string? filePath = null)
        {
            await Locator(ContactUsPageSelectors.NameInput).FillAsync(name);
            await Locator(ContactUsPageSelectors.EmailInput).FillAsync(email);
            await Locator(ContactUsPageSelectors.SubjectInput).FillAsync(subject);
            await Locator(ContactUsPageSelectors.MessageInput).FillAsync(message);

            if (!string.IsNullOrEmpty(filePath))
            {
                await Locator(ContactUsPageSelectors.UploadFileInput).SetInputFilesAsync(filePath);
            }

            void DialogHandler(object? sender, IDialog dialog)
            {
                _ = AcceptDialogAsync(dialog);
            }

            static async Task AcceptDialogAsync(IDialog dialog)
            {
                try
                {
                    await dialog.AcceptAsync();
                }
                catch (PlaywrightException ex)
                {
                    TestLog.Warn($"Failed to accept confirmation dialog: {ex.Message}");
                }
            }

            Page.Dialog += DialogHandler;

            try
            {
                await Locator(ContactUsPageSelectors.SubmitButton).ClickAsync();
            }
            finally
            {
                Page.Dialog -= DialogHandler;
            }
        }

        public async Task<bool> IsSuccessAlertVisibleAsync()
        {
            return await IsVisibleAfterWaitAsync(ContactUsPageSelectors.SuccessAlert);
        }

        public async Task ClickReturnHomeAsync()
        {
            await Locator(ContactUsPageSelectors.ReturnHomeButton).ClickAsync();
        }
    }
}
