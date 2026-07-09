using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTests.Constants;

namespace ApiTests.Clients
{
    public class NotesApiClient
    {
        private readonly ApiClient _apiClient;

        public NotesApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IAPIResponse> CreateNoteAsync(string title, string description, string category)
        {
            var form = new Dictionary<string, object>
            {
                { "title", title },
                { "description", description },
                { "category", category }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.Notes, form);
        }

        public async Task<IAPIResponse> GetAllNotesAsync()
        {
            return await _apiClient.GetAsync(ApiEndpoints.Notes);
        }

        public async Task<IAPIResponse> GetNoteByIdAsync(string id)
        {
            return await _apiClient.GetAsync(ApiEndpoints.NoteById(id));
        }

        public async Task<IAPIResponse> UpdateNoteAsync(string id, string title, string description, bool completed, string category)
        {
            var form = new Dictionary<string, object>
            {
                { "title", title },
                { "description", description },
                { "completed", completed },
                { "category", category }
            };
            return await _apiClient.PutFormAsync(ApiEndpoints.NoteById(id), form);
        }

        public async Task<IAPIResponse> UpdateNoteCompletedStatusAsync(string id, bool completed)
        {
            var form = new Dictionary<string, object>
            {
                { "completed", completed }
            };
            return await _apiClient.PatchFormAsync(ApiEndpoints.NoteById(id), form);
        }

        public async Task<IAPIResponse> DeleteNoteAsync(string id)
        {
            return await _apiClient.DeleteAsync(ApiEndpoints.NoteById(id));
        }
    }
}
