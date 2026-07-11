using NUnit.Framework;
using System.Threading.Tasks;
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
    [AllureSubSuite("Products and Reviews")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Products")]
    public class ProductTests : BaseTest
    {
        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 8: Verify All Products and product detail page")]
        public async Task ProductDetails_ShouldDisplayCorrectFields()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var detailsPage = new ProductDetailsPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and open Products page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
                Assert.IsTrue(await productsPage.IsProductsHeaderVisibleAsync("ALL PRODUCTS"), "ALL PRODUCTS header not visible.");
            });

            await AllureHelper.StepAsync("Verify product list is visible", async () =>
            {
                var count = await productsPage.GetProductListCountAsync();
                Assert.IsTrue(count > 0, "No products visible in the list.");
            });

            await AllureHelper.StepAsync("Click on 'View Product' for the first product", async () =>
            {
                await productsPage.ClickProductDetailsByIdAsync(1);
            });

            await AllureHelper.StepAsync("Verify product fields: Name, Category, Price, Availability, Condition, Brand", async () =>
            {
                var name = await detailsPage.GetProductNameAsync();
                var category = await detailsPage.GetProductCategoryAsync();
                var price = await detailsPage.GetProductPriceAsync();
                var availability = await detailsPage.GetProductAvailabilityAsync();
                var condition = await detailsPage.GetProductConditionAsync();
                var brand = await detailsPage.GetProductBrandAsync();

                Assert.IsFalse(string.IsNullOrEmpty(name), "Product name is empty.");
                Assert.IsTrue(category.Contains("Category:"), "Category field not found or incorrect.");
                Assert.IsTrue(price.Contains("Rs."), "Price field not found or incorrect.");
                Assert.IsTrue(availability.Contains("Availability:"), "Availability field not found.");
                Assert.IsTrue(condition.Contains("Condition:"), "Condition field not found.");
                Assert.IsTrue(brand.Contains("Brand:"), "Brand field not found.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 9: Search Product")]
        public async Task SearchProduct_ShouldDisplayMatchingProducts()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);

            var searchTerm = "Blue Top";

            await AllureHelper.StepAsync("Navigate to home page and open Products page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
            });

            await AllureHelper.StepAsync("Search for the product and verify results page header", async () =>
            {
                await productsPage.SearchProductAsync(searchTerm);
                Assert.IsTrue(await productsPage.IsProductsHeaderVisibleAsync("SEARCHED PRODUCTS"), "SEARCHED PRODUCTS header not visible.");
            });

            await AllureHelper.StepAsync("Verify searched product is visible in the list", async () =>
            {
                Assert.IsTrue(await productsPage.IsProductVisibleInListAsync(searchTerm), $"Product '{searchTerm}' not found in search results.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Test Case 21: Add review on product")]
        public async Task SubmitProductReview_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var detailsPage = new ProductDetailsPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and open Products page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
            });

            await AllureHelper.StepAsync("Open first product details", async () =>
            {
                await productsPage.ClickProductDetailsByIdAsync(1);
            });

            await AllureHelper.StepAsync("Fill review details and submit", async () =>
            {
                await detailsPage.SubmitReviewAsync("QA Reviewer", "reviewer@gmail.com", "This is an automated review feedback for this product. Extremely high quality!");
            });

            await AllureHelper.StepAsync("Verify review success message is shown", async () =>
            {
                Assert.IsTrue(await detailsPage.IsReviewSuccessAlertVisibleAsync(), "Review success alert is not visible.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 18: View Category Products")]
        public async Task ViewCategoryProducts_ShouldShowCorrectProducts()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and verify categories panel is visible", async () =>
            {
                await homePage.NavigateAsync();
                Assert.IsTrue(await Page.IsVisibleAsync(".category-products"), "Categories panel is not visible.");
            });

            await AllureHelper.StepAsync("Click on 'Women' category and subcategory 'Dress'", async () =>
            {
                await productsPage.ClickCategoryWomenAsync();
                await productsPage.ClickCategoryWomenDressAsync();
            });

            await AllureHelper.StepAsync("Verify Category page header contains 'WOMEN - DRESS PRODUCTS'", async () =>
            {
                var title = await productsPage.GetProductsTitleTextAsync();
                Assert.IsTrue(title.Contains("WOMEN - DRESS PRODUCTS", System.StringComparison.OrdinalIgnoreCase), $"Expected title to contain 'WOMEN - DRESS PRODUCTS', but was '{title}'");
            });

            await AllureHelper.StepAsync("Click on 'Men' category and subcategory 'Tshirts'", async () =>
            {
                await productsPage.ClickCategoryMenAsync();
                await productsPage.ClickCategoryMenTshirtsAsync();
            });

            await AllureHelper.StepAsync("Verify navigated to category page containing 'MEN - TSHIRTS PRODUCTS'", async () =>
            {
                var title = await productsPage.GetProductsTitleTextAsync();
                Assert.IsTrue(title.Contains("MEN - TSHIRTS PRODUCTS", System.StringComparison.OrdinalIgnoreCase), $"Expected title to contain 'MEN - TSHIRTS PRODUCTS', but was '{title}'");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 19: View & Cart Brand Products")]
        public async Task ViewAndCartBrandProducts_ShouldShowCorrectBrandProducts()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);

            await AllureHelper.StepAsync("Navigate to home page, open Products page, and verify brands panel is visible", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
                Assert.IsTrue(await Page.IsVisibleAsync(".brands_products"), "Brands panel is not visible.");
            });

            await AllureHelper.StepAsync("Click on 'Babyhug' brand", async () =>
            {
                await productsPage.ClickBrandAsync("Babyhug");
            });

            await AllureHelper.StepAsync("Verify navigated to brand page and brand products are displayed", async () =>
            {
                var title = await productsPage.GetProductsTitleTextAsync();
                Assert.IsTrue(title.Contains("BRAND - BABYHUG PRODUCTS", System.StringComparison.OrdinalIgnoreCase), $"Expected title to contain 'BRAND - BABYHUG PRODUCTS', but was '{title}'");
                Assert.IsTrue(await productsPage.GetProductListCountAsync() > 0, "No products visible for Babyhug brand.");
            });

            await AllureHelper.StepAsync("Click on another brand 'Kookie Kids'", async () =>
            {
                await productsPage.ClickBrandAsync("Kookie Kids");
            });

            await AllureHelper.StepAsync("Verify navigated to that brand page and brand products are displayed", async () =>
            {
                var title = await productsPage.GetProductsTitleTextAsync();
                Assert.IsTrue(title.Contains("BRAND - KOOKIE KIDS PRODUCTS", System.StringComparison.OrdinalIgnoreCase), $"Expected title to contain 'BRAND - KOOKIE KIDS PRODUCTS', but was '{title}'");
                Assert.IsTrue(await productsPage.GetProductListCountAsync() > 0, "No products visible for Kookie Kids brand.");
            });
        }
    }
}