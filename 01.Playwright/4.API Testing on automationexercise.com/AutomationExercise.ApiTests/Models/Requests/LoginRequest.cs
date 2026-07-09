using System.Collections.Generic;

namespace AutomationExercise.ApiTests.Models.Requests
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public Dictionary<string, object> ToFormData()
        {
            var data = new Dictionary<string, object>();
            if (Email != null) data.Add("email", Email);
            if (Password != null) data.Add("password", Password);
            return data;
        }
    }
}
