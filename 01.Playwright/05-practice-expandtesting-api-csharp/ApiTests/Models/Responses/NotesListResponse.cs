using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiTests.Models.Responses
{
    public class NotesListResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public List<NoteData> Data { get; set; } = new();
    }
}
