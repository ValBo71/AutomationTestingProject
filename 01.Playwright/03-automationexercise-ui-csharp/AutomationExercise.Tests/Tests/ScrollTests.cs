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
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Page Navigation & Scrolling")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Scrolling")]
    public class ScrollTests : BaseTest
    {
        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 25: Verify Scroll Up using 'Arrow' button and Scroll Down")]
        public async Task ScrollUp_WithArrowButton_ShouldSucceed()
        {
            var homePage = new HomePage(Page);

            await AllureHelper.StepAsync("Navigate to home page and verify visibility", async () =>
            {
                await homePage.NavigateAsync();
                Assert.IsTrue(await Page.IsVisibleAsync("a[href='/']"), "Home page is not visible.");
            });

            await AllureHelper.StepAsync("Scroll down to the bottom and verify Subscription is visible", async () =>
            {
                await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Page.WaitForSelectorAsync("#subscribe", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                Assert.IsTrue(await Page.IsVisibleAsync("#subscribe"), "Subscription form is not visible at the bottom.");
            });

            await AllureHelper.StepAsync("Click on scroll up arrow button and verify page scrolled up", async () =>
            {
                await Page.ClickAsync("#scrollUp");
                // Wait for scroll transition to top
                await Page.WaitForFunctionAsync("window.scrollY === 0", null, new() { Timeout = 3000 });
                
                var headerLocator = Page.Locator("section#slider h2:has-text('Full-Fledged practice website')").First;
                await headerLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                Assert.IsTrue(await headerLocator.IsVisibleAsync(), "Header text is not visible at the top after scroll up.");
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 26: Verify Scroll Up without 'Arrow' button and Scroll Down")]
        public async Task ScrollUp_WithoutArrowButton_ShouldSucceed()
        {
            var homePage = new HomePage(Page);

            await AllureHelper.StepAsync("Navigate to home page and verify visibility", async () =>
            {
                await homePage.NavigateAsync();
                Assert.IsTrue(await Page.IsVisibleAsync("a[href='/']"), "Home page is not visible.");
            });

            await AllureHelper.StepAsync("Scroll down to the bottom and verify Subscription is visible", async () =>
            {
                await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Page.WaitForSelectorAsync("#subscribe", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                Assert.IsTrue(await Page.IsVisibleAsync("#subscribe"), "Subscription form is not visible at the bottom.");
            });

            await AllureHelper.StepAsync("Scroll up to top of page and verify header is visible", async () =>
            {
                await Page.EvaluateAsync("window.scrollTo(0, 0)");
                // Wait for scroll transition to top
                await Page.WaitForFunctionAsync("window.scrollY === 0", null, new() { Timeout = 3000 });
                
                var headerLocator = Page.Locator("section#slider h2:has-text('Full-Fledged practice website')").First;
                await headerLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                Assert.IsTrue(await headerLocator.IsVisibleAsync(), "Header text is not visible at the top after scrolling up.");
            });
        }
    }
}
