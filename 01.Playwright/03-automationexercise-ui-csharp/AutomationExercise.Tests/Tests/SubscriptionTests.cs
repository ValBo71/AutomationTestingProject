using NUnit.Framework;
using System.Threading.Tasks;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SeverityLevel = Allure.Net.Commons.SeverityLevel;

namespace AutomationExercise.Tests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Footer Subscription")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Subscription")]
    public class SubscriptionTests : BaseTest
    {
        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 10: Verify Subscription in home page")]
        public async Task Subscription_OnHomePage_ShouldSubscribeSuccessfully()
        {
            var homePage = new HomePage(Page);
            var randomEmail = RandomDataGenerator.GenerateEmail();

            await AllureHelper.StepAsync("Navigate to home page", async () =>
            {
                await homePage.NavigateAsync();
            });

            await AllureHelper.StepAsync("Scroll to footer, input email, and subscribe", async () =>
            {
                await homePage.SubscribeAsync(randomEmail);
                Assert.IsTrue(await homePage.IsSubscriptionSuccessVisibleAsync(), "Subscription success message not shown on home page.");
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 11: Verify Subscription in Cart page")]
        public async Task Subscription_OnCartPage_ShouldSubscribeSuccessfully()
        {
            var homePage = new HomePage(Page);
            var cartPage = new CartPage(Page);
            var randomEmail = RandomDataGenerator.GenerateEmail();

            await AllureHelper.StepAsync("Navigate to home page and open Cart page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickCartAsync();
            });

            await AllureHelper.StepAsync("Scroll to footer, input email, and subscribe from Cart page", async () =>
            {
                await homePage.SubscribeAsync(randomEmail);
                Assert.IsTrue(await homePage.IsSubscriptionSuccessVisibleAsync(), "Subscription success message not shown on Cart page.");
            });
        }
    }
}