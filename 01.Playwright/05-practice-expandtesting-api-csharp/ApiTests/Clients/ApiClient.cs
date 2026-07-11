using Microsoft.Playwright;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiTests.Helpers;

namespace ApiTests.Clients
{
    public class ApiClient
    {
        protected readonly IAPIRequestContext Request;
        private string? _token;

        public ApiClient(IAPIRequestContext request)
        {
            Request = request;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public void ClearToken()
        {
            _token = null;
        }

        private APIRequestContextOptions GetOptions(Dictionary<string, object>? formData = null, object? jsonBody = null, Dictionary<string, string>? customHeaders = null)
        {
            var options = new APIRequestContextOptions();
            var headers = new Dictionary<string, string>();

            if (customHeaders != null)
            {
                foreach (var h in customHeaders)
                {
                    headers[h.Key] = h.Value;
                }
            }

            if (!string.IsNullOrEmpty(_token))
            {
                headers["x-auth-token"] = _token;
            }

            if (headers.Count > 0)
            {
                options.Headers = headers;
            }

            if (formData != null)
            {
                var form = Request.CreateFormData();
                foreach (var kvp in formData)
                {
                    var valStr = kvp.Value is bool b ? (b ? "true" : "false") : (kvp.Value?.ToString() ?? string.Empty);
                    form.Set(kvp.Key, valStr);
                }
                options.Form = form;
            }

            if (jsonBody != null)
            {
                options.DataObject = jsonBody;
            }

            return options;
        }

        public async Task<IAPIResponse> GetAsync(string endpoint, Dictionary<string, object>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(customHeaders: headers);
            if (queryParams != null)
            {
                options.Params = queryParams;
            }

            var response = await Request.GetAsync(endpoint, options);
            
            // Log the actual resolved URL from the Playwright response so query values are properly encoded
            AllureHelper.AttachRequest("GET", response.Url);
            
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }


        public async Task<IAPIResponse> PostFormAsync(string endpoint, Dictionary<string, object> formData, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(formData: formData, customHeaders: headers);
            var sb = new StringBuilder();
            foreach (var kvp in formData)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }

            AllureHelper.AttachRequest("POST", endpoint, body: sb.ToString());
            var response = await Request.PostAsync(endpoint, options);
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }

        public async Task<IAPIResponse> PostJsonAsync(string endpoint, object jsonBody, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(jsonBody: jsonBody, customHeaders: headers);
            var bodyStr = JsonHelper.Serialize(jsonBody);

            AllureHelper.AttachRequest("POST", endpoint, body: bodyStr);
            var response = await Request.PostAsync(endpoint, options);
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }

        public async Task<IAPIResponse> PutFormAsync(string endpoint, Dictionary<string, object> formData, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(formData: formData, customHeaders: headers);
            var sb = new StringBuilder();
            foreach (var kvp in formData)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }

            AllureHelper.AttachRequest("PUT", endpoint, body: sb.ToString());
            var response = await Request.PutAsync(endpoint, options);
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }

        public async Task<IAPIResponse> PatchFormAsync(string endpoint, Dictionary<string, object> formData, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(formData: formData, customHeaders: headers);
            var sb = new StringBuilder();
            foreach (var kvp in formData)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }

            AllureHelper.AttachRequest("PATCH", endpoint, body: sb.ToString());
            var response = await Request.PatchAsync(endpoint, options);
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }

        public async Task<IAPIResponse> DeleteAsync(string endpoint, Dictionary<string, string>? headers = null)
        {
            var options = GetOptions(customHeaders: headers);
            AllureHelper.AttachRequest("DELETE", endpoint);
            var response = await Request.DeleteAsync(endpoint, options);
            var body = await response.TextAsync();
            AllureHelper.AttachResponse(response.Status, body: body);
            return response;
        }
    }
}
