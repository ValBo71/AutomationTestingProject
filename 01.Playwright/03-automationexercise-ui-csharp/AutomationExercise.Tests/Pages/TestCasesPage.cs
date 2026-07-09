using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class TestCasesPage
    {
        private readonly IPage _page;

        public TestCasesPage(IPage page)
        {
            _page = page;
        }

        private const string TestCasesHeader = "h2.title";

        public async Task<bool> IsTestCasesPageLoadedAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForURLAsync("**/test_cases");
            return await _page.IsVisibleAsync(TestCasesHeader);
        }
    }
}
