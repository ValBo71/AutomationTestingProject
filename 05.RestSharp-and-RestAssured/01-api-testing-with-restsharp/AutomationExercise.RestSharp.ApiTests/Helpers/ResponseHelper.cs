using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Helpers
{
    public static class ResponseHelper
    {
        public static T Deserialize<T>(RestResponse response)
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "Response content is null or empty");
            try
            {
                var result = JsonSerializer.Deserialize<T>(response.Content!, JsonHelper.DefaultOptions);
                Assert.That(result, Is.Not.Null, $"Failed to deserialize response to {typeof(T).Name}");
                return result!;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception occurred during deserialization to {typeof(T).Name}: {ex.Message}. Content was: {response.Content}");
                throw;
            }
        }

        public static int GetResponseCode(RestResponse response)
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            var node = JsonNode.Parse(response.Content!);
            Assert.That(node, Is.Not.Null);
            var codeNode = node!["responseCode"];
            Assert.That(codeNode, Is.Not.Null, "JSON response does not contain 'responseCode' property.");
            return codeNode!.GetValue<int>();
        }

        public static string GetMessage(RestResponse response)
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            var node = JsonNode.Parse(response.Content!);
            Assert.That(node, Is.Not.Null);
            var msgNode = node!["message"];
            Assert.That(msgNode, Is.Not.Null, "JSON response does not contain 'message' property.");
            return msgNode!.GetValue<string>();
        }

        public static string FormatJson(string json)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(document, JsonHelper.DefaultOptions);
            }
            catch
            {
                return json; // Fallback to raw string if it's not valid JSON
            }
        }
    }
}
