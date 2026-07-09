namespace AutomationExercise.Tests.Selectors
{
    public static class ContactUsPageSelectors
    {
        public const string NameInput = "input[data-qa='name']";
        public const string EmailInput = "input[data-qa='email']";
        public const string SubjectInput = "input[data-qa='subject']";
        public const string MessageInput = "textarea[data-qa='message']";
        public const string UploadFileInput = "input[name='upload_file']";
        public const string SubmitButton = "input[data-qa='submit-button']";
        public const string SuccessAlert = "div.status.alert-success"; // text matches success
        public const string ReturnHomeButton = "a.btn-success";
    }
}
