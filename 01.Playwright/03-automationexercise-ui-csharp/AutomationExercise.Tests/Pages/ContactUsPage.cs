using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class ContactUsPage
    {
        private readonly IPage _page;

        public ContactUsPage(IPage page)
        {
            _page = page;
        }

        public async Task SubmitContactFormAsync(
            string name,
            string email,
            string subject,
            string message,
            string? filePath = null)
        {
            await _page.FillAsync(ContactUsPageSelectors.NameInput, name);
            await _page.FillAsync(ContactUsPageSelectors.EmailInput, email);
            await _page.FillAsync(ContactUsPageSelectors.SubjectInput, subject);
            await _page.FillAsync(ContactUsPageSelectors.MessageInput, message);

            if (!string.IsNullOrEmpty(filePath))
            {
                await _page.SetInputFilesAsync(ContactUsPageSelectors.UploadFileInput, filePath);
            }

            // Bind click handler to accept confirm dialog
            _page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync();
            };

            await _page.ClickWithOverloadCheckAsync(ContactUsPageSelectors.SubmitButton);
        }

        public async Task<bool> IsSuccessAlertVisibleAsync()
        {
            await _page.WaitForSelectorAsync(ContactUsPageSelectors.SuccessAlert);
            return await _page.IsVisibleAsync(ContactUsPageSelectors.SuccessAlert);
        }

        public async Task ClickReturnHomeAsync()
        {
            await _page.ClickWithOverloadCheckAsync(ContactUsPageSelectors.ReturnHomeButton);
        }
    }
}
