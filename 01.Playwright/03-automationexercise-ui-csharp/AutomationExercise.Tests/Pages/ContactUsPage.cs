using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

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
                dialog.AcceptAsync().ConfigureAwait(false);
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
            var successAlert = Locator(ContactUsPageSelectors.SuccessAlert);
            await successAlert.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await successAlert.IsVisibleAsync();
        }

        public async Task ClickReturnHomeAsync()
        {
            await Locator(ContactUsPageSelectors.ReturnHomeButton).ClickAsync();
        }
    }
}
