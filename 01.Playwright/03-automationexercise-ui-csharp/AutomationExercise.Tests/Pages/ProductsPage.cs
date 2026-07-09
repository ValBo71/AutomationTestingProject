using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.Pages
{
    public class ProductsPage
    {
        private readonly IPage _page;

        public ProductsPage(IPage page)
        {
            _page = page;
        }

        public async Task SearchProductAsync(string productName)
        {
            await _page.FillAsync(ProductsPageSelectors.SearchInput, productName);
            await _page.ClickWithOverloadCheckAsync(ProductsPageSelectors.SearchButton);
        }

        public async Task<bool> IsProductsHeaderVisibleAsync(string headerText)
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ProductsHeader);
            var text = await _page.InnerTextAsync(ProductsPageSelectors.ProductsHeader);
            return text.Contains(headerText);
        }

        public async Task<int> GetProductListCountAsync()
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ProductListItems);
            var items = await _page.QuerySelectorAllAsync(ProductsPageSelectors.ProductListItems);
            return items.Count;
        }

        public async Task ClickFirstProductDetailsAsync()
        {
            await ClickProductDetailsByIdAsync(1);
        }
        
        public async Task ClickProductDetailsByIdAsync(int productId)
        {
            await _page.ClickWithOverloadCheckAsync($"a[href='/product_details/{productId}']");
        }

        public async Task AddFirstProductToCartAsync()
        {
            // Hover over first product and click Add to Cart
            var locator = _page.Locator("a.add-to-cart[data-product-id='1']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }

        public async Task AddSecondProductToCartAsync()
        {
            var locator = _page.Locator("a.add-to-cart[data-product-id='2']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }

        public async Task AddProductToCartByIdAsync(int productId)
        {
            var locator = _page.Locator($"a.add-to-cart[data-product-id='{productId}']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }

        public async Task ClickModalViewCartAsync()
        {
            await _page.ClickWithOverloadCheckAsync(ProductsPageSelectors.ModalViewCartButton);
        }

        public async Task ClickModalContinueShoppingAsync()
        {
            await _page.ClickAsync(ProductsPageSelectors.ModalContinueShoppingButton);
        }

        public async Task<bool> IsProductVisibleInListAsync(string productName)
        {
            await _page.WaitForSelectorAsync(ProductsPageSelectors.ProductListItems);
            var text = await _page.InnerTextAsync(ProductsPageSelectors.ProductListItems);
            return text.Contains(productName);
        }

        public async Task ClickCategoryWomenAsync()
        {
            await _page.ClickWithOverloadCheckAsync("a[href='#Women']");
        }

        public async Task ClickCategoryWomenDressAsync()
        {
            await _page.ClickWithOverloadCheckAsync("a[href='/category_products/1']");
        }

        public async Task ClickCategoryMenAsync()
        {
            await _page.ClickWithOverloadCheckAsync("a[href='#Men']");
        }

        public async Task ClickCategoryMenTshirtsAsync()
        {
            await _page.ClickWithOverloadCheckAsync("a[href='/category_products/3']");
        }

        public async Task<string> GetProductsTitleTextAsync()
        {
            await _page.WaitForSelectorAsync("h2.title.text-center");
            return await _page.InnerTextAsync("h2.title.text-center");
        }

        public async Task ClickBrandAsync(string brandName)
        {
            await _page.ClickWithOverloadCheckAsync($"a[href='/brand_products/{brandName}']");
        }
    }
}