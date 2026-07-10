using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Base;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.Models.Requests;
using AutomationExercise.ApiTests.Models.Responses;
using AutomationExercise.ApiTests.TestData;

namespace AutomationExercise.ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Account & Auth")]
    [AllureFeature("User Profile Management")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "Integration", "Account")]
    public class AccountApiTests : BaseApiTest
    {
        private AccountApiClient _accountClient = null!;
        private CreateUserRequest _testUser = null!;
        private bool _isUserCreated;

        [SetUp]
        public void TestSetUp()
        {
            _accountClient = new AccountApiClient(RequestContext);
        }

        [TearDown]
        public async Task TestTearDown()
        {
            // Fallback Cleanup: If the user was created but not deleted by the test sequence, delete it.
            if (_isUserCreated && _testUser != null)
            {
                try
                {
                    await _accountClient.DeleteAccountAsync(_testUser.Email, _testUser.Password);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[TearDown Cleanup Warning] Failed to delete user {_testUser.Email} during fallback cleanup: {ex.Message}");
                }
            }
        }

        [Test]
        [AllureName("API 11 & 14 & 13 & 12: Complete User Lifecycle Integration Test")]
        [AllureDescription("Execute complete integration flow: Create user (API 11) -> Get details (API 14) -> Update details (API 13) -> Delete user (API 12).")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task CompleteUserLifecycle_ShouldCreateUpdateAndRetrieveAndCleanUpUser()
        {
            // -------------------------------------------------------------
            // Step 1: Create Account (API 11)
            // -------------------------------------------------------------
            _testUser = TestUsers.GenerateRegisterUserRequest();

            var createResponse = await _accountClient.CreateAccountAsync(_testUser);

            Assert.That(createResponse.Status, Is.EqualTo(200), "HTTP Status for Create Account should be 200 OK");
            
            var createBody = await createResponse.TextAsync();
            var createMsgNode = JsonHelper.Deserialize<ApiMessageResponse>(createBody);

            Assert.That(createMsgNode, Is.Not.Null);
            Assert.That(createMsgNode!.ResponseCode, Is.EqualTo(201), "API response code for Account Creation should be 201");
            Assert.That(createMsgNode.Message, Is.EqualTo(ExpectedMessages.UserCreated));
            
            _isUserCreated = true;

            // -------------------------------------------------------------
            // Step 2: Get Account Details by Email (API 14)
            // -------------------------------------------------------------
            var getResponse = await _accountClient.GetUserDetailByEmailAsync(_testUser.Email);

            Assert.That(getResponse.Status, Is.EqualTo(200), "HTTP Status for Get Details should be 200 OK");

            var getBody = await getResponse.TextAsync();
            var getDetailsNode = JsonHelper.Deserialize<UserAccountResponse>(getBody);

            Assert.That(getDetailsNode, Is.Not.Null);
            Assert.That(getDetailsNode!.ResponseCode, Is.EqualTo(200));
            Assert.That(getDetailsNode.User, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(getDetailsNode.User.Email, Is.EqualTo(_testUser.Email));
                Assert.That(getDetailsNode.User.Name, Is.EqualTo(_testUser.Name));
                Assert.That(getDetailsNode.User.FirstName, Is.EqualTo(_testUser.FirstName));
                Assert.That(getDetailsNode.User.LastName, Is.EqualTo(_testUser.LastName));
                Assert.That(getDetailsNode.User.Company, Is.EqualTo(_testUser.Company));
                Assert.That(getDetailsNode.User.City, Is.EqualTo(_testUser.City));
            });

            // -------------------------------------------------------------
            // Step 3: Update Account (API 13)
            // -------------------------------------------------------------
            var updateRequest = new UpdateUserRequest
            {
                Name = _testUser.Name + "_Updated",
                Email = _testUser.Email,
                Password = _testUser.Password,
                Title = _testUser.Title,
                BirthDate = _testUser.BirthDate,
                BirthMonth = _testUser.BirthMonth,
                BirthYear = _testUser.BirthYear,
                FirstName = _testUser.FirstName + "_Updated",
                LastName = _testUser.LastName + "_Updated",
                Company = _testUser.Company + "_Updated",
                Address1 = _testUser.Address1 + "_Updated",
                Address2 = _testUser.Address2 + "_Updated",
                Country = _testUser.Country,
                Zipcode = _testUser.Zipcode,
                State = _testUser.State,
                City = _testUser.City + "_Updated",
                MobileNumber = _testUser.MobileNumber
            };

            var updateResponse = await _accountClient.UpdateAccountAsync(updateRequest);

            Assert.That(updateResponse.Status, Is.EqualTo(200), "HTTP Status for Update Account should be 200 OK");

            var updateBody = await updateResponse.TextAsync();
            var updateMsgNode = JsonHelper.Deserialize<ApiMessageResponse>(updateBody);

            Assert.That(updateMsgNode, Is.Not.Null);
            Assert.That(updateMsgNode!.ResponseCode, Is.EqualTo(200), "API response code for Account Update should be 200");
            Assert.That(updateMsgNode.Message, Is.EqualTo(ExpectedMessages.UserUpdated));

            // Verify update via GET details
            var getResponseAfterUpdate = await _accountClient.GetUserDetailByEmailAsync(_testUser.Email);
            var getBodyAfterUpdate = await getResponseAfterUpdate.TextAsync();
            var getDetailsNodeAfterUpdate = JsonHelper.Deserialize<UserAccountResponse>(getBodyAfterUpdate);

            Assert.That(getDetailsNodeAfterUpdate, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(getDetailsNodeAfterUpdate!.User.Name, Is.EqualTo(updateRequest.Name));
                Assert.That(getDetailsNodeAfterUpdate.User.FirstName, Is.EqualTo(updateRequest.FirstName));
                Assert.That(getDetailsNodeAfterUpdate.User.LastName, Is.EqualTo(updateRequest.LastName));
                Assert.That(getDetailsNodeAfterUpdate.User.Company, Is.EqualTo(updateRequest.Company));
                Assert.That(getDetailsNodeAfterUpdate.User.City, Is.EqualTo(updateRequest.City));
            });

            // -------------------------------------------------------------
            // Step 4: Delete Account (API 12)
            // -------------------------------------------------------------
            var deleteResponse = await _accountClient.DeleteAccountAsync(_testUser.Email, _testUser.Password);

            Assert.That(deleteResponse.Status, Is.EqualTo(200), "HTTP Status for Delete Account should be 200 OK");

            var deleteBody = await deleteResponse.TextAsync();
            var deleteMsgNode = JsonHelper.Deserialize<ApiMessageResponse>(deleteBody);

            Assert.That(deleteMsgNode, Is.Not.Null);
            Assert.That(deleteMsgNode!.ResponseCode, Is.EqualTo(200), "API response code for Account Deletion should be 200");
            Assert.That(deleteMsgNode.Message, Is.EqualTo(ExpectedMessages.AccountDeleted));

            _isUserCreated = false; // Mark as cleaned up

            // -------------------------------------------------------------
            // Step 5: Verify account is deleted (read details should return 404/user not found)
            // -------------------------------------------------------------
            var getResponseAfterDelete = await _accountClient.GetUserDetailByEmailAsync(_testUser.Email);
            var getBodyAfterDelete = await getResponseAfterDelete.TextAsync();
            var getDetailsNodeAfterDelete = JsonHelper.Deserialize<ApiMessageResponse>(getBodyAfterDelete);

            Assert.That(getDetailsNodeAfterDelete, Is.Not.Null);
            Assert.That(getDetailsNodeAfterDelete!.ResponseCode, Is.EqualTo(404), "GET details after deletion should return 404 inner responseCode");
            Assert.That(getDetailsNodeAfterDelete.Message, Is.EqualTo(ExpectedMessages.AccountNotFound));
        }
    }
}
