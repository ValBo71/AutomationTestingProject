using Microsoft.Playwright;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Helpers;

namespace AutomationExercise.ApiTests.Clients
{
    public class ApiClient
    {
        protected readonly IAPIRequestContext Request;

        public ApiClient(IAPIRequestContext request)
        {
            Request = request;
        }

        protected async Task<IAPIResponse> ExecuteGetAsync(string endpoint, Dictionary<string, object>? queryParams = null)
        {
            var options = new APIRequestContextOptions();
            if (queryParams != null)
            {
                options.Params = queryParams;
            }

            var url = endpoint;
            if (queryParams != null)
            {
                var sb = new StringBuilder();
                foreach (var param in queryParams)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.Append($"{param.Key}={param.Value}");
                }
                url += sb.ToString();
            }

            AllureHelper.AttachRequest("GET", url);
            
            var response = await Request.GetAsync(endpoint, options);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);

            return response;
        }

        protected async Task<IAPIResponse> ExecutePostAsync(string endpoint, Dictionary<string, object>? formData = null)
        {
            var options = new APIRequestContextOptions();
            string? bodyString = null;
            if (formData != null)
            {
                var form = Request.CreateFormData();
                var sb = new StringBuilder();
                foreach (var kvp in formData)
                {
                    var valStr = kvp.Value?.ToString() ?? string.Empty;
                    form.Set(kvp.Key, valStr);
                    sb.AppendLine($"{kvp.Key}: {valStr}");
                }
                options.Form = form;
                bodyString = sb.ToString();
            }

            AllureHelper.AttachRequest("POST", endpoint, body: bodyString);
            
            var response = await Request.PostAsync(endpoint, options);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);

            return response;
        }

        protected async Task<IAPIResponse> ExecutePutAsync(string endpoint, Dictionary<string, object>? formData = null)
        {
            var options = new APIRequestContextOptions();
            string? bodyString = null;
            if (formData != null)
            {
                var form = Request.CreateFormData();
                var sb = new StringBuilder();
                foreach (var kvp in formData)
                {
                    var valStr = kvp.Value?.ToString() ?? string.Empty;
                    form.Set(kvp.Key, valStr);
                    sb.AppendLine($"{kvp.Key}: {valStr}");
                }
                options.Form = form;
                bodyString = sb.ToString();
            }

            AllureHelper.AttachRequest("PUT", endpoint, body: bodyString);
            
            var response = await Request.PutAsync(endpoint, options);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);

            return response;
        }

        protected async Task<IAPIResponse> ExecuteDeleteAsync(string endpoint, Dictionary<string, object>? formData = null)
        {
            var options = new APIRequestContextOptions();
            string? bodyString = null;
            if (formData != null)
            {
                var form = Request.CreateFormData();
                var sb = new StringBuilder();
                foreach (var kvp in formData)
                {
                    var valStr = kvp.Value?.ToString() ?? string.Empty;
                    form.Set(kvp.Key, valStr);
                    sb.AppendLine($"{kvp.Key}: {valStr}");
                }
                options.Form = form;
                bodyString = sb.ToString();
            }

            AllureHelper.AttachRequest("DELETE", endpoint, body: bodyString);
            
            var response = await Request.DeleteAsync(endpoint, options);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);

            return response;
        }
    }
}
