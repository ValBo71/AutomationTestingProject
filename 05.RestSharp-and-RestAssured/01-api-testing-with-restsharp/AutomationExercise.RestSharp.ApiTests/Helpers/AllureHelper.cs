using System;
using System.Linq;
using Allure.Net.Commons;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Helpers
{
    public static class AllureHelper
    {
        public static void AttachRequest(RestRequest request, string? baseUrl)
        {
            var method = request.Method.ToString();
            var fullUrl = $"{baseUrl?.TrimEnd('/')}/{request.Resource.TrimStart('/')}";
            
            var headersString = string.Join("\n", request.Parameters
                .Where(p => p.Type == ParameterType.HttpHeader)
                .Select(p => $"{p.Name}: {p.Value}"));

            var bodyString = "";
            var formParams = request.Parameters
                .Where(p => p.Type == ParameterType.GetOrPost)
                .Select(p => $"{p.Name}={p.Value}");
            
            if (formParams.Any())
            {
                bodyString = "Form parameters:\n" + string.Join("\n", formParams);
            }
            else
            {
                var bodyParam = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
                if (bodyParam != null)
                {
                    bodyString = bodyParam.Value?.ToString() ?? "";
                }
            }

            var requestInfo = $"Method: {method}\n" +
                              $"URL: {fullUrl}\n\n" +
                              $"Headers:\n{headersString}\n\n" +
                              $"Body:\n{bodyString}";

            AllureApi.AddAttachment("API Request", "text/plain", System.Text.Encoding.UTF8.GetBytes(requestInfo), ".txt");
        }

        public static void AttachResponse(RestResponse response)
        {
            var statusCode = response.StatusCode.ToString();
            var headersString = string.Join("\n", response.Headers?
                .Select(h => $"{h.Name}: {h.Value}") ?? Array.Empty<string>());
            
            var body = response.Content ?? "";
            var formattedBody = ResponseHelper.FormatJson(body);

            var responseInfo = $"Status Code: {statusCode}\n\n" +
                               $"Headers:\n{headersString}\n\n" +
                               $"Response Body:\n{formattedBody}";

            AllureApi.AddAttachment("API Response", "text/plain", System.Text.Encoding.UTF8.GetBytes(responseInfo), ".txt");
        }
    }
}
