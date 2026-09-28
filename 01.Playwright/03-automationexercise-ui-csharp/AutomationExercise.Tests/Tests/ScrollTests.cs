using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Playwright;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace AutomationExercise.Tests.Tests
{
    /// <summary>
    /// Scrolling is asserted with ToBeInViewportAsync, not IsVisibleAsync. "Visible" in Playwright means
    /// rendered with a size - an element far below the fold is visible too - so the earlier checks held
    /// without any scrolling at all. Being inside the viewport is what a scroll actually changes.
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Page Navigation & Scrolling")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Scrolling")]
    public class ScrollTests : BaseTest
    {
        private const string HeaderText = "Full-Fledged practice website for Automation Engineers";

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 25: Verify Scroll Up using 'Arrow' button and Scroll Down")]
        public async Task ScrollUp_WithArrowButton_ShouldSucceed()
        {
            var homePage = new HomePage(Page);

            await AllureHelper.StepAsync("Navigate to home page and verify it is shown", async () =>
            {
                await homePage.NavigateAsync();
                Assert.IsTrue(await homePage.IsHomePageShownAsync(), "Home page is not shown.");
            });

            await AllureHelper.StepAsync("Scroll down to the bottom and verify SUBSCRIPTION is on screen", async () =>
            {
                await Assertions.Expect(homePage.SubscriptionHeading()).Not.ToBeInViewportAsync();
                await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Assertions.Expect(homePage.SubscriptionHeading()).ToBeInViewportAsync();
            });

            await AllureHelper.StepAsync("Click the scroll-up arrow and verify the top of the page is on screen", async () =>
            {
                await Assertions.Expect(homePage.ActiveSlideHeading()).Not.ToBeInViewportAsync();
                await Page.ClickAsync("#scrollUp");

                // The arrow animates the scroll, so the assertion's own retrying covers the transition.
                await Assertions.Expect(homePage.ActiveSlideHeading()).ToBeInViewportAsync();
                await Assertions.Expect(homePage.ActiveSlideHeading()).ToContainTextAsync(HeaderText);
                await Assertions.Expect(homePage.SubscriptionHeading()).Not.ToBeInViewportAsync();
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 26: Verify Scroll Up without 'Arrow' button and Scroll Down")]
        public async Task ScrollUp_WithoutArrowButton_ShouldSucceed()
        {
            var homePage = new HomePage(Page);

            await AllureHelper.StepAsync("Navigate to home page and verify it is shown", async () =>
            {
                await homePage.NavigateAsync();
                Assert.IsTrue(await homePage.IsHomePageShownAsync(), "Home page is not shown.");
            });

            await AllureHelper.StepAsync("Scroll down to the bottom and verify SUBSCRIPTION is on screen", async () =>
            {
                await Assertions.Expect(homePage.SubscriptionHeading()).Not.ToBeInViewportAsync();
                await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Assertions.Expect(homePage.SubscriptionHeading()).ToBeInViewportAsync();
            });

            await AllureHelper.StepAsync("Scroll up to the top and verify the header text is on screen", async () =>
            {
                // Scrolling by script is what this test case asks for ("without the arrow"). What the test
                // checks is the page's response to it: the heading comes back into view.
                await Assertions.Expect(homePage.ActiveSlideHeading()).Not.ToBeInViewportAsync();
                await Page.EvaluateAsync("window.scrollTo(0, 0)");
                await Assertions.Expect(homePage.ActiveSlideHeading()).ToBeInViewportAsync();
                await Assertions.Expect(homePage.ActiveSlideHeading()).ToContainTextAsync(HeaderText);
            });
        }
    }
}
