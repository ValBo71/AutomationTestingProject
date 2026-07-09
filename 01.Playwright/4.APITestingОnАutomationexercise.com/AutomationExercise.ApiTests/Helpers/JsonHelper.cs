using System.Text.Json;

namespace AutomationExercise.ApiTests.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, Options);
        }
    }
}
