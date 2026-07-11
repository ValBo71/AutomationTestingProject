using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class ProductDetailsPage : BasePage
    {
        public ProductDetailsPage(IPage page) : base(page)
        {
        }

        public async Task<string> GetProductNameAsync()
        {
            var detailName = Locator(ProductsPageSelectors.DetailProductName);
            await detailName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailName.InnerTextAsync();
        }

        public async Task<string> GetProductCategoryAsync()
        {
            var detailCategory = Locator(ProductsPageSelectors.DetailProductCategory);
            await detailCategory.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailCategory.InnerTextAsync();
        }

        public async Task<string> GetProductPriceAsync()
        {
            var detailPrice = Locator(ProductsPageSelectors.DetailProductPrice);
            await detailPrice.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailPrice.InnerTextAsync();
        }

        public async Task<string> GetProductAvailabilityAsync()
        {
            var detailAvailability = Locator(ProductsPageSelectors.DetailProductAvailability);
            await detailAvailability.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailAvailability.InnerTextAsync();
        }

        public async Task<string> GetProductConditionAsync()
        {
            var detailCondition = Locator(ProductsPageSelectors.DetailProductCondition);
            await detailCondition.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailCondition.InnerTextAsync();
        }

        public async Task<string> GetProductBrandAsync()
        {
            var detailBrand = Locator(ProductsPageSelectors.DetailProductBrand);
            await detailBrand.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await detailBrand.InnerTextAsync();
        }

        public async Task SetQuantityAsync(int quantity)
        {
            await Locator(ProductsPageSelectors.DetailProductQuantityInput).FillAsync(quantity.ToString());
        }

        public async Task ClickAddToCartAsync()
        {
            await Locator(ProductsPageSelectors.DetailProductAddToCartButton).ClickAsync();
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task SubmitReviewAsync(string name, string email, string reviewText)
        {
            var nameInput = Locator(ProductsPageSelectors.ReviewNameInput);
            await nameInput.ScrollIntoViewIfNeededAsync();
            await nameInput.FillAsync(name);
            await Locator(ProductsPageSelectors.ReviewEmailInput).FillAsync(email);
            await Locator(ProductsPageSelectors.ReviewTextInput).FillAsync(reviewText);
            await Locator(ProductsPageSelectors.ReviewSubmitButton).ClickAsync();
        }

        public async Task<bool> IsReviewSuccessAlertVisibleAsync()
        {
            var successAlert = Locator(ProductsPageSelectors.ReviewSuccessAlert);
            await successAlert.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await successAlert.IsVisibleAsync();
        }
    }
}