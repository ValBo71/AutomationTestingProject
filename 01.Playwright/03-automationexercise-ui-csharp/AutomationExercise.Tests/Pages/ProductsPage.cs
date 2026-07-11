using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.Tests.Selectors;

namespace AutomationExercise.Tests.Pages
{
    public class ProductsPage : BasePage
    {
        public ProductsPage(IPage page) : base(page)
        {
        }

        public async Task SearchProductAsync(string productName)
        {
            await Locator(ProductsPageSelectors.SearchInput).FillAsync(productName);
            await Locator(ProductsPageSelectors.SearchButton).ClickAsync();
        }

        public async Task<bool> IsProductsHeaderVisibleAsync(string headerText)
        {
            var header = Locator(ProductsPageSelectors.ProductsHeader);
            await header.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var text = await header.InnerTextAsync();
            return text.Contains(headerText);
        }

        public async Task<int> GetProductListCountAsync()
        {
            var items = Locator(ProductsPageSelectors.ProductListItems);
            await items.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await items.CountAsync();
        }

        public async Task ClickFirstProductDetailsAsync()
        {
            await ClickProductDetailsByIdAsync(1);
        }
        
        public async Task ClickProductDetailsByIdAsync(int productId)
        {
            await Locator($"a[href='/product_details/{productId}']").ClickAsync();
        }

        public async Task AddFirstProductToCartAsync()
        {
            var locator = Locator("a.add-to-cart[data-product-id='1']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task AddSecondProductToCartAsync()
        {
            var locator = Locator("a.add-to-cart[data-product-id='2']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task AddProductToCartByIdAsync(int productId)
        {
            var locator = Locator($"a.add-to-cart[data-product-id='{productId}']").First;
            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task ClickModalViewCartAsync()
        {
            await Locator(ProductsPageSelectors.ModalViewCartButton).ClickAsync();
        }

        public async Task ClickModalContinueShoppingAsync()
        {
            await Locator(ProductsPageSelectors.ModalContinueShoppingButton).ClickAsync();
        }

        public async Task<bool> IsProductVisibleInListAsync(string productName)
        {
            var items = Locator(ProductsPageSelectors.ProductListItems);
            await items.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var text = await items.InnerTextAsync();
            return text.Contains(productName);
        }

        public async Task ClickCategoryWomenAsync()
        {
            await Locator("a[href='#Women']").ClickAsync();
        }

        public async Task ClickCategoryWomenDressAsync()
        {
            await Locator("a[href='/category_products/1']").ClickAsync();
        }

        public async Task ClickCategoryMenAsync()
        {
            await Locator("a[href='#Men']").ClickAsync();
        }

        public async Task ClickCategoryMenTshirtsAsync()
        {
            await Locator("a[href='/category_products/3']").ClickAsync();
        }

        public async Task<string> GetProductsTitleTextAsync()
        {
            var title = Locator("h2.title.text-center");
            await title.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            return await title.InnerTextAsync();
        }

        public async Task ClickBrandAsync(string brandName)
        {
            await Locator($"a[href='/brand_products/{brandName}']").ClickAsync();
        }
    }
}