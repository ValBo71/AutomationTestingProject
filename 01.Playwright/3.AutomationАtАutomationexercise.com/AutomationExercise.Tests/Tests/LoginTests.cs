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
    [AllureSubSuite("User Authentication")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Login")]
    public class LoginTests : BaseTest
    {
        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 2: Login User with correct email and password")]
        public async Task LoginUser_WithCorrectCredentials_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomEmail = RandomDataGenerator.GenerateEmail();
            var username = "LoginUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);
            var password = "Password123!";

            await AllureHelper.StepAsync("Navigate to home page and register user", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomEmail);
                await signupPage.FillSignupDetailsAsync(
                    password: password,
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Login",
                    lastName: "User",
                    address1: "123 Login St",
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90001",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                
                // Logout to clear the session so we can test Login
                await homePage.ClickLogoutAsync();
            });

            await AllureHelper.StepAsync("Perform login with valid credentials", async () =>
            {
                await loginPage.LoginAsync(randomEmail, password);
            });

            await AllureHelper.StepAsync("Verify logged in status and user name", async () =>
            {
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Logged in username is not visible.");
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 3: Login User with incorrect email and password")]
        public async Task LoginUser_WithIncorrectCredentials_ShouldShowError()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and open Login page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
            });

            await AllureHelper.StepAsync("Attempt login with invalid credentials and verify error", async () =>
            {
                await loginPage.LoginAsync("invalid_user_1234@gmail.com", "WrongPassword123!");
                var errorText = await loginPage.GetLoginErrorTextAsync();
                Assert.AreEqual("Your email or password is incorrect!", errorText, "Incorrect login failure message.");
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 4: Logout User")]
        public async Task LogoutUser_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomEmail = RandomDataGenerator.GenerateEmail();
            var username = "LogoutUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);
            var password = "Password123!";

            await AllureHelper.StepAsync("Register user and log out", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomEmail);
                await signupPage.FillSignupDetailsAsync(
                    password: password,
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Logout",
                    lastName: "User",
                    address1: "123 Logout St",
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90001",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                await homePage.ClickLogoutAsync();
            });

            await AllureHelper.StepAsync("Navigate to home page and log in", async () =>
            {
                await homePage.ClickLoginSignupAsync();
                await loginPage.LoginAsync(randomEmail, password);
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Logged in username not visible.");
            });

            await AllureHelper.StepAsync("Logout and verify user is redirected to Login page", async () =>
            {
                await homePage.ClickLogoutAsync();
                Assert.IsTrue(await loginPage.IsSignupFormVisibleAsync(), "Not redirected to Login/Signup page after logout.");
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await loginPage.LoginAsync(randomEmail, password);
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }
    }
}