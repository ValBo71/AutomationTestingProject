using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ApiTests.Helpers
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

        public static async Task<T> DeserializeAsync<T>(IAPIResponse response)
        {
            var body = await response.TextAsync();
            Assert.That(body, Is.Not.Null.And.Not.Empty, $"Response body was empty (status {response.Status}).");

            T? result;
            try
            {
                result = JsonSerializer.Deserialize<T>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException ex)
            {
                throw new JsonException(
                    $"Failed to deserialize response body as {typeof(T).Name} (status {response.Status}). Body: {body}",
                    ex);
            }

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Deserializing response body as {typeof(T).Name} produced null (status {response.Status}). Body: {body}");
            }

            return result;
        }

        public static async Task<bool> IsValidJsonAsync(IAPIResponse response)
        {
            try
            {
                var content = await response.TextAsync();
                using var doc = JsonDocument.Parse(content);
                return true;
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
        }
    }
}
