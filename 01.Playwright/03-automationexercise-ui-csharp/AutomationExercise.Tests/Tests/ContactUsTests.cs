using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Playwright;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using System.IO;
using System;

namespace AutomationExercise.Tests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Contact Us & Info")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "ContactUs")]
    public class ContactUsTests : BaseTest
    {
        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 6: Contact Us Form")]
        public async Task ContactUsForm_ShouldSubmitSuccessfully()
        {
            var homePage = new HomePage(Page);
            var contactPage = new ContactUsPage(Page);

            // Create a dummy file to upload
            var testFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
            Directory.CreateDirectory(testFileDir);
            var testFilePath = Path.Combine(testFileDir, "test_upload.txt");
            File.WriteAllText(testFilePath, "This is a test upload file contents for Contact Us form submission.");

            await AllureHelper.StepAsync("Navigate to home page and open Contact Us page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickContactUsAsync();
            });

            await AllureHelper.StepAsync("Submit the contact form including file upload and accept dialog", async () =>
            {
                await contactPage.SubmitContactFormAsync(
                    name: "QA Support",
                    email: "support_qa@gmail.com",
                    subject: "Bug Report: Checkout failing",
                    message: "Dear support team, checkout page seems to throw javascript console errors during pay confirmation. Please review.",
                    filePath: testFilePath
                );
            });

            await AllureHelper.StepAsync("Verify contact form success alert is shown", async () =>
            {
                Assert.IsTrue(await contactPage.IsSuccessAlertVisibleAsync(), "Contact Us form success alert is not visible.");
            });

            await AllureHelper.StepAsync("Click return home button and verify homepage loading", async () =>
            {
                await contactPage.ClickReturnHomeAsync();
                await Page.WaitForSelectorAsync("a[href='/']", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                Assert.IsTrue(await Page.IsVisibleAsync("a[href='/']"), "Failed to return to Home Page.");
                
                // Cleanup temp file
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 7: Verify Test Cases page")]
        public async Task VerifyTestCasesPage_ShouldLoadSuccessfully()
        {
            var homePage = new HomePage(Page);
            var testCasesPage = new TestCasesPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and open Test Cases page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickTestCasesAsync();
            });

            await AllureHelper.StepAsync("Verify user is navigated to test cases page successfully", async () =>
            {
                Assert.IsTrue(await testCasesPage.IsTestCasesPageLoadedAsync(), "Test Cases page was not loaded successfully.");
            });
        }
    }
}