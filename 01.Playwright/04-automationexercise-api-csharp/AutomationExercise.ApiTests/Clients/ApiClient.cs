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

            var response = await Request.GetAsync(endpoint, options);

            AllureHelper.AttachRequest("GET", response.Url);
            
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

            var response = await Request.PostAsync(endpoint, options);

            AllureHelper.AttachRequest("POST", response.Url, body: bodyString);
            
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

            var response = await Request.PutAsync(endpoint, options);

            AllureHelper.AttachRequest("PUT", response.Url, body: bodyString);
            
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

            var response = await Request.DeleteAsync(endpoint, options);

            AllureHelper.AttachRequest("DELETE", response.Url, body: bodyString);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);

            return response;
        }
    }
}
