namespace AutomationExercise.Tests.Selectors
{
    public static class LoginPageSelectors
    {
        // Login Form Selectors
        public const string LoginEmailInput = "input[data-qa='login-email']";
        public const string LoginPasswordInput = "input[data-qa='login-password']";
        public const string LoginButton = "button[data-qa='login-button']";
        public const string LoginErrorMessage = "form[action='/login'] p"; // e.g. "Your email or password is incorrect!"
        
        // Signup Form Selectors
        public const string SignupNameInput = "input[data-qa='signup-name']";
        public const string SignupEmailInput = "input[data-qa='signup-email']";
        public const string SignupButton = "button[data-qa='signup-button']";
        public const string SignupErrorMessage = "form[action='/signup'] p"; // e.g. "Email Address already exist!"
    }
}
