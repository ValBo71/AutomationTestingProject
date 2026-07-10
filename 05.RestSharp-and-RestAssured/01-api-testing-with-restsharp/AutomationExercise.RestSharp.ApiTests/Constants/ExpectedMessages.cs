namespace AutomationExercise.RestSharp.ApiTests.Constants
{
    public static class ExpectedMessages
    {
        public const string MethodNotSupported = "This request method is not supported.";
        public const string SearchProductMissing = "Bad request, search_product parameter is missing in POST request.";
        public const string UserExists = "User exists!";
        public const string LoginParametersMissing = "Bad request, email or password parameter is missing in POST request.";
        public const string UserNotFound = "User not found!";
        public const string UserCreated = "User created!";
        public const string AccountDeleted = "Account deleted!";
        public const string UserUpdated = "User updated!";
        public const string AccountNotFound = "Account not found with this email, try another email!";
    }
}
