using NUnit.Framework;
using System.Threading.Tasks;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using AutomationExercise.Tests.TestData;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace AutomationExercise.Tests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Account Management")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Account")]
    public class AccountTests : BaseTest
    {
        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 1: Register User")]
        public async Task RegisterUser_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomUser = RandomDataGenerator.GenerateEmail();
            var username = "TestUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);

            await AllureHelper.StepAsync("Navigate to home page and open Signup/Login page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                Assert.IsTrue(await loginPage.IsSignupFormVisibleAsync(), "Signup / Login page was not loaded successfully.");
            });

            await AllureHelper.StepAsync("Fill initial signup details (Name and Email)", async () =>
            {
                await loginPage.SignUpInitAsync(username, randomUser);
            });

            await AllureHelper.StepAsync("Fill full signup form and submit", async () =>
            {
                await signupPage.FillSignupDetailsAsync(
                    password: "Password123!",
                    day: "15",
                    month: "May",
                    year: "1990",
                    firstName: "Test",
                    lastName: "User",
                    address1: "123 Main St",
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90001",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
            });

            await AllureHelper.StepAsync("Verify 'ACCOUNT CREATED!' message is shown", async () =>
            {
                Assert.IsTrue(await createdPage.IsAccountCreatedVisibleAsync(), "'ACCOUNT CREATED!' header not visible.");
                await createdPage.ClickContinueAsync();
                AccountPendingCleanup = true;
            });

            await AllureHelper.StepAsync("Verify 'Logged in as username' is visible", async () =>
            {
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Logged in user text is not visible or name is incorrect.");
            });

            await AllureHelper.StepAsync("Delete account and verify 'ACCOUNT DELETED!' message is shown", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                Assert.IsTrue(await deletedPage.IsAccountDeletedVisibleAsync(), "'ACCOUNT DELETED!' header not visible.");
                await deletedPage.ClickContinueAsync();
                AccountPendingCleanup = false;
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 5: Register User with existing email")]
        public async Task RegisterUser_WithExistingEmail_ShouldShowError()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var duplicateEmail = RandomDataGenerator.GenerateEmail();
            var username = "DupUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);
            var password = "Password123!";

            await AllureHelper.StepAsync("Register a user first with the email", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, duplicateEmail);
                await signupPage.FillSignupDetailsAsync(
                    password: password,
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Dup",
                    lastName: "User",
                    address1: "123 Dup St",
                    country: "United States",
                    state: "Texas",
                    city: "Austin",
                    zipcode: "73301",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                AccountPendingCleanup = true;

                // Logout to clear session
                await homePage.ClickLogoutAsync();
            });

            await AllureHelper.StepAsync("Attempt to sign up again with the same email and verify error", async () =>
            {
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, duplicateEmail);
                var errorText = await loginPage.GetSignupErrorTextAsync();
                Assert.AreEqual("Email Address already exist!", errorText, "Incorrect error message for duplicate email signup.");
            });

            await AllureHelper.StepAsync("Clean up: log back in as the first user and delete the account", async () =>
            {
                await loginPage.LoginAsync(duplicateEmail, password);
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
                AccountPendingCleanup = false;
            });
        }
    }
}