namespace AutomationExercise.Tests.Selectors
{
    public static class CommonSelectors
    {
        public const string NavigationHome = "a[href='/']";
        public const string NavigationProducts = "a[href='/products']";
        public const string NavigationCart = "a[href='/view_cart']";
        public const string NavigationSignupLogin = "a[href='/login']";
        public const string NavigationContactUs = "a[href='/contact_us']";
        public const string NavigationTestCases = "a[href='/test_cases']";
        public const string NavigationLogout = "a[href='/logout']";
        public const string NavigationDeleteAccount = "a[href='/delete_account']";
        public const string LoggedInUserText = "li:has(a:has-text('Logged in as'))";
    }
}
