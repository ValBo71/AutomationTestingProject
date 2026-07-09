using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class ProductDetailsPage
    {
        private readonly IPage _page;

        public ProductDetailsPage(IPage page)
        {
            _page = page;
        }

        public async Task<string> GetProductNameAsync()
        {
            await _page.CheckAndReloadIfOverloadedAsync();
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductName);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductName);
        }

        public async Task<string> GetProductCategoryAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductCategory);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductCategory);
        }

        public async Task<string> GetProductPriceAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductPrice);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductPrice);
        }

        public async Task<string> GetProductAvailabilityAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductAvailability);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductAvailability);
        }

        public async Task<string> GetProductConditionAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductCondition);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductCondition);
        }

        public async Task<string> GetProductBrandAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.DetailProductBrand);
            return await _page.InnerTextAsync(ProductsPageSelectors.DetailProductBrand);
        }

        public async Task SetQuantityAsync(int quantity)
        {
            await _page.FillAsync(ProductsPageSelectors.DetailProductQuantityInput, quantity.ToString());
        }

        public async Task ClickAddToCartAsync()
        {
            await _page.ClickAsync(ProductsPageSelectors.DetailProductAddToCartButton);
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }

        public async Task SubmitReviewAsync(string name, string email, string reviewText)
        {
            await _page.Locator(ProductsPageSelectors.ReviewNameInput).ScrollIntoViewIfNeededAsync();
            await _page.FillAsync(ProductsPageSelectors.ReviewNameInput, name);
            await _page.FillAsync(ProductsPageSelectors.ReviewEmailInput, email);
            await _page.FillAsync(ProductsPageSelectors.ReviewTextInput, reviewText);
            await _page.ClickAsync(ProductsPageSelectors.ReviewSubmitButton);
        }

        public async Task<bool> IsReviewSuccessAlertVisibleAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ReviewSuccessAlert);
            return await _page.IsVisibleAsync(ProductsPageSelectors.ReviewSuccessAlert);
        }
    }
}