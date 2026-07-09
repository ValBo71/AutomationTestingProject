namespace ApiTests.Constants
{
    public static class ApiEndpoints
    {
        public const string HealthCheck = "health-check";
        
        public const string UsersRegister = "users/register";
        public const string UsersLogin = "users/login";
        public const string UsersProfile = "users/profile";
        public const string UsersForgotPassword = "users/forgot-password";
        public const string UsersVerifyResetPasswordToken = "users/verify-reset-password-token";
        public const string UsersResetPassword = "users/reset-password";
        public const string UsersChangePassword = "users/change-password";
        public const string UsersLogout = "users/logout";
        public const string UsersDeleteAccount = "users/delete-account";

        public const string Notes = "notes";

        public static string NoteById(string id)
        {
            return $"notes/{id}";
        }
    }
}
