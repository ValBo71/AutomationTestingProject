using Allure.Net.Commons;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ApiTests.Helpers
{
    public static class AllureHelper
    {
        public static void AttachRequest(string method, string url, string? headers = null, string? body = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Method: {method}");
            sb.AppendLine($"URL: {url}");
            if (!string.IsNullOrEmpty(headers))
            {
                sb.AppendLine("Headers:");
                sb.AppendLine(headers);
            }
            if (!string.IsNullOrEmpty(body))
            {
                sb.AppendLine("Body / Parameters:");
                sb.AppendLine(body);
            }

            var contentBytes = Encoding.UTF8.GetBytes(sb.ToString());
            AllureApi.AddAttachment("API Request Details", "text/plain", contentBytes, ".txt");
        }

        public static void AttachResponse(int statusCode, string? headers = null, string? body = null)
        {
            var contentBytes = Encoding.UTF8.GetBytes(body ?? string.Empty);
            AllureApi.AddAttachment("API Response Details", "application/json", contentBytes, ".json");
        }

        public static void LogStep(string stepName, Action action)
        {
            AllureApi.Step(stepName, action);
        }

        public static async Task LogStepAsync(string stepName, Func<Task> action)
        {
            await AllureApi.Step(stepName, action);
        }
    }
}
