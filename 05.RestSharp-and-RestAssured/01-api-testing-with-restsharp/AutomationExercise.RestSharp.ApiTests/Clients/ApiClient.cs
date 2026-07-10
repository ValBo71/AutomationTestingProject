using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationExercise.RestSharp.ApiTests.Helpers;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Clients
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(RestClient client)
        {
            _client = client;
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            var response = await _client.ExecuteAsync(request);

            // Log details in Allure Report
            AllureHelper.AttachRequest(request, _client.Options.BaseUrl?.ToString());
            AllureHelper.AttachResponse(response);

            return response;
        }

        public async Task<RestResponse> GetAsync(string resource, Dictionary<string, string>? queryParams = null)
        {
            var request = new RestRequest(resource, Method.Get);
            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }
            return await ExecuteAsync(request);
        }

        public async Task<RestResponse> PostFormAsync(string resource, Dictionary<string, string>? formParams = null)
        {
            var request = new RestRequest(resource, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            if (formParams != null)
            {
                foreach (var param in formParams)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }
            return await ExecuteAsync(request);
        }

        public async Task<RestResponse> PutFormAsync(string resource, Dictionary<string, string>? formParams = null)
        {
            var request = new RestRequest(resource, Method.Put);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            if (formParams != null)
            {
                foreach (var param in formParams)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }
            return await ExecuteAsync(request);
        }

        public async Task<RestResponse> DeleteFormAsync(string resource, Dictionary<string, string>? formParams = null)
        {
            var request = new RestRequest(resource, Method.Delete);
            if (formParams != null)
            {
                var contentParts = new List<string>();
                foreach (var param in formParams)
                {
                    contentParts.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                }
                var bodyContent = string.Join("&", contentParts);
                request.AddStringBody(bodyContent, "application/x-www-form-urlencoded");
            }
            return await ExecuteAsync(request);
        }
    }
}
