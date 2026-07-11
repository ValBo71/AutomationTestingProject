using Microsoft.Playwright;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Pages
{
    public class TestCasesPage : BasePage
    {
        public TestCasesPage(IPage page) : base(page)
        {
        }

        private const string TestCasesHeader = "h2.title";

        public async Task<bool> IsTestCasesPageLoadedAsync()
        {
            await Page.WaitForURLAsync("**/test_cases");
            return await IsVisibleAfterWaitAsync(TestCasesHeader);
        }
    }
}
