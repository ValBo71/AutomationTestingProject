namespace AutomationExercise.Tests.Selectors
{
    public static class CommonSelectors
    {
        public const string NavigationHome = "#header a[href='/']";
        public const string NavigationProducts = "#header a[href='/products']";
        public const string NavigationCart = "#header a[href='/view_cart']";
        public const string NavigationSignupLogin = "#header a[href='/login']";
        public const string NavigationContactUs = "#header a[href='/contact_us']";
        public const string NavigationTestCases = "#header a[href='/test_cases']";
        public const string NavigationLogout = "#header a[href='/logout']";
        public const string NavigationDeleteAccount = "#header a[href='/delete_account']";
        public const string LoggedInUserText = "li:has(a:has-text('Logged in as'))";
    }
}
