using NUnit.Framework;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using System.Threading.Tasks;
using Microsoft.Playwright;
using ApiTests.Base;
using ApiTests.Helpers;
using ApiTests.Models.Responses;
using ApiTests.Constants;

namespace ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("API Tests")]
    [AllureSubSuite("Notes")]
    [AllureTag("API", "Playwright", "Notes")]
    [AllureOwner("QA Automation")]
    public class NotesApiTests : BaseApiTest
    {
        private string _userEmail = string.Empty;
        private string _userPassword = string.Empty;

        [SetUp]
        public async Task TestSetUp()
        {
            _userEmail = RandomDataGenerator.GenerateUniqueEmail();
            _userPassword = "Password123";
            var name = RandomDataGenerator.GenerateRandomString("User", 6);

            await UsersClient.RegisterAsync(name, _userEmail, _userPassword);
            var loginResponse = await UsersClient.LoginAsync(_userEmail, _userPassword);
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
            
            BaseClient.SetToken(loginData.Data.Token);
        }

        [TearDown]
        public async Task TestTearDown()
        {
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
        }

        [Test]
        [AllureName("Note Lifecycle Integration Flow")]
        [AllureDescription("Verify full CRUD note lifecycle: Create -> Get -> Update -> Delete.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task NoteCRUDLifecycle_ShouldSucceed()
        {
            string noteId = string.Empty;
            var title = "Practice Playwright API";
            var desc = "Write integration test suite for Expand Testing";
            var category = "Work";

            // 1. Create
            var createResponse = await NotesClient.CreateNoteAsync(title, desc, category);
            Assert.That(createResponse.Status, Is.EqualTo(200));
            var noteObj = await ResponseHelper.DeserializeAsync<NoteResponse>(createResponse);
            Assert.That(noteObj.Success, Is.True);
            Assert.That(noteObj.Data.Title, Is.EqualTo(title));
            Assert.That(noteObj.Data.Description, Is.EqualTo(desc));
            Assert.That(noteObj.Data.Category, Is.EqualTo(category));
            Assert.That(noteObj.Data.Completed, Is.False);
            noteId = noteObj.Data.Id;

            // 2. Get by ID
            var getResponse = await NotesClient.GetNoteByIdAsync(noteId);
            Assert.That(getResponse.Status, Is.EqualTo(200));
            var fetchedNote = await ResponseHelper.DeserializeAsync<NoteResponse>(getResponse);
            Assert.That(fetchedNote.Data.Id, Is.EqualTo(noteId));
            Assert.That(fetchedNote.Data.Title, Is.EqualTo(title));

            // 3. Update (PUT)
            var newTitle = "Updated Practice Playwright API";
            var newDesc = "Updated description";
            var updateResponse = await NotesClient.UpdateNoteAsync(noteId, newTitle, newDesc, true, "Personal");
            Assert.That(updateResponse.Status, Is.EqualTo(200));
            var updatedNote = await ResponseHelper.DeserializeAsync<NoteResponse>(updateResponse);
            Assert.That(updatedNote.Message, Is.EqualTo(ExpectedMessages.NoteUpdatedSuccess));
            Assert.That(updatedNote.Data.Title, Is.EqualTo(newTitle));
            Assert.That(updatedNote.Data.Description, Is.EqualTo(newDesc));
            Assert.That(updatedNote.Data.Completed, Is.True);
            Assert.That(updatedNote.Data.Category, Is.EqualTo("Personal"));

            // 4. Update status (PATCH)
            var patchResponse = await NotesClient.UpdateNoteCompletedStatusAsync(noteId, false);
            Assert.That(patchResponse.Status, Is.EqualTo(200));
            var patchedNote = await ResponseHelper.DeserializeAsync<NoteResponse>(patchResponse);
            Assert.That(patchedNote.Data.Completed, Is.False);

            // 5. Delete
            var deleteResponse = await NotesClient.DeleteNoteAsync(noteId);
            Assert.That(deleteResponse.Status, Is.EqualTo(200));
            var deleteResult = await ResponseHelper.DeserializeAsync<GenericResponse>(deleteResponse);
            Assert.That(deleteResult.Success, Is.True);

            // 6. Verify Deleted
            var verifyGetResponse = await NotesClient.GetNoteByIdAsync(noteId);
            Assert.That(verifyGetResponse.Status, Is.EqualTo(400).Or.EqualTo(404));
        }

        [TestCase("Home")]
        [TestCase("Work")]
        [TestCase("Personal")]
        [AllureName("Create note - Valid Categories Parameterized")]
        [AllureDescription("Verify creating notes under each of the valid categories (Home, Work, Personal).")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task CreateNote_WithValidCategories_ShouldCreateNote(string category)
        {
            var title = "Title for " + category;
            var desc = "Description text";

            var response = await NotesClient.CreateNoteAsync(title, desc, category);
            Assert.That(response.Status, Is.EqualTo(200));

            var note = await ResponseHelper.DeserializeAsync<NoteResponse>(response);
            Assert.That(note.Success, Is.True);
            Assert.That(note.Data.Category, Is.EqualTo(category));
        }

        [Test]
        [AllureName("Create note - Invalid Category Negative Scenario")]
        [AllureDescription("Verify that creating a note with an unsupported category returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task CreateNote_WithInvalidCategory_ShouldReturnBadRequest()
        {
            var response = await NotesClient.CreateNoteAsync("Title", "Description", "InvalidCategory");
            Assert.That(response.Status, Is.EqualTo(400));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(response);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Get notes list - Positive Scenario")]
        [AllureDescription("Verify retrieving list of notes for authenticated user.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetAllNotes_ShouldReturnList()
        {
            await NotesClient.CreateNoteAsync("Note A", "Desc A", "Personal");
            await NotesClient.CreateNoteAsync("Note B", "Desc B", "Work");

            var getResponse = await NotesClient.GetAllNotesAsync();
            Assert.That(getResponse.Status, Is.EqualTo(200));
            var notesList = await ResponseHelper.DeserializeAsync<NotesListResponse>(getResponse);
            Assert.That(notesList.Success, Is.True);
            Assert.That(notesList.Data.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        [AllureName("Create note - Missing Required Fields Negative Scenario")]
        [AllureDescription("Verify creating a note with missing title returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task CreateNote_WithMissingTitle_ShouldReturnBadRequest()
        {
            var createResponse = await NotesClient.CreateNoteAsync("", "Description only", "Home");
            Assert.That(createResponse.Status, Is.EqualTo(400));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(createResponse);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Create note - Unauthorized Negative Scenario")]
        [AllureDescription("Verify that requests without authorization token return 401 Unauthorized.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task CreateNote_WithoutToken_ShouldReturnUnauthorized()
        {
            BaseClient.ClearToken();

            var createResponse = await NotesClient.CreateNoteAsync("Title", "Description", "Home");
            Assert.That(createResponse.Status, Is.EqualTo(401));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(createResponse);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Get Note - Non-Existent ID Negative Scenario")]
        [AllureDescription("Verify that retrieving a note by a non-existent ID returns 400 or 404.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetNoteById_WithNonExistentId_ShouldReturnNotFound()
        {
            var fakeId = "64189a74314e9f0218a5213f"; // Valid BSON ObjectId format but non-existent
            var response = await NotesClient.GetNoteByIdAsync(fakeId);
            Assert.That(response.Status, Is.EqualTo(400).Or.EqualTo(404));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(response);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Get Note - Invalid ID Format Negative Scenario")]
        [AllureDescription("Verify that retrieving a note with an invalid ID format returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task GetNoteById_WithInvalidIdFormat_ShouldReturnBadRequest()
        {
            var invalidId = "invalid_id_123";
            var response = await NotesClient.GetNoteByIdAsync(invalidId);
            Assert.That(response.Status, Is.EqualTo(400));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(response);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Delete Note - Non-Existent ID Negative Scenario")]
        [AllureDescription("Verify that deleting a note with a non-existent ID returns 400 or 404.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task DeleteNote_WithNonExistentId_ShouldReturnNotFound()
        {
            var fakeId = "64189a74314e9f0218a5213f";
            var response = await NotesClient.DeleteNoteAsync(fakeId);
            Assert.That(response.Status, Is.EqualTo(400).Or.EqualTo(404));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(response);
            Assert.That(body.Success, Is.False);
        }
    }
}
