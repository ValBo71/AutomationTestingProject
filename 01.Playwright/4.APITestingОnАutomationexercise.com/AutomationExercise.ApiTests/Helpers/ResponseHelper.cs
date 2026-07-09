using Microsoft.Playwright;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AutomationExercise.ApiTests.Helpers
{
    public static class ResponseHelper
    {
        public static async Task<string> GetResponseBodyAsync(IAPIResponse response)
        {
            return await response.TextAsync();
        }

        public static async Task<JsonNode?> ParseToJsonNodeAsync(IAPIResponse response)
        {
            var content = await response.TextAsync();
            return JsonNode.Parse(content);
        }

        public static async Task<int?> GetResponseCodeAsync(IAPIResponse response)
        {
            var json = await ParseToJsonNodeAsync(response);
            return json?["responseCode"]?.GetValue<int>();
        }

        public static async Task<string?> GetMessageAsync(IAPIResponse response)
        {
            var json = await ParseToJsonNodeAsync(response);
            return json?["message"]?.GetValue<string>();
        }
    }
}
