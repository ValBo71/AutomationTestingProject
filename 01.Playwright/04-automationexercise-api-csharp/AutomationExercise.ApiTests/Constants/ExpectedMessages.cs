namespace AutomationExercise.ApiTests.Constants
{
    public static class ExpectedMessages
    {
        public const string MethodNotSupported = "This request method is not supported.";
        public const string UserExists = "User exists!";
        public const string UserNotFound = "User not found!";
        public const string AccountNotFound = "Account not found with this email, try another email!";
        public const string EmailMissing = "Bad request, email or password parameter is missing in POST request.";
        public const string SearchParamMissing = "Bad request, search_product parameter is missing in POST request.";
        public const string UserCreated = "User created!";
        public const string UserUpdated = "User updated!";
        public const string AccountDeleted = "Account deleted!";
    }
}
