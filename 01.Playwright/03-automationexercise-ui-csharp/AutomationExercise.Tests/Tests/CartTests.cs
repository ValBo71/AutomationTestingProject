using NUnit.Framework;
using System.Threading.Tasks;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using AutomationExercise.Tests.TestData;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace AutomationExercise.Tests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Cart Operations")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Cart")]
    public class CartTests : BaseTest
    {
        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 12: Add Products in Cart")]
        public async Task AddProductsToCart_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and open Products page", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
            });

            await AllureHelper.StepAsync("Add first product to cart and click Continue Shopping", async () =>
            {
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalContinueShoppingAsync();
            });

            await AllureHelper.StepAsync("Add second product to cart and click View Cart", async () =>
            {
                await productsPage.AddSecondProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
            });

            await AllureHelper.StepAsync("Verify both products are in the cart", async () =>
            {
                var count = await cartPage.GetCartItemCountAsync();
                Assert.AreEqual(2, count, "Cart does not contain exactly 2 items.");
            });

            await AllureHelper.StepAsync("Verify quantities of both items are 1", async () =>
            {
                var quantity1 = await cartPage.GetCartItemQuantityAsync(1);
                var quantity2 = await cartPage.GetCartItemQuantityAsync(2);

                Assert.AreEqual(1, quantity1, "First item quantity in cart is not 1.");
                Assert.AreEqual(1, quantity2, "Second item quantity in cart is not 1.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 13: Verify Product quantity in Cart")]
        public async Task VerifyProductQuantityInCart_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var detailsPage = new ProductDetailsPage(Page);
            var cartPage = new CartPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and view details of first product", async () =>
            {
                await homePage.NavigateAsync();
                await productsPage.ClickProductDetailsByIdAsync(1);
            });

            await AllureHelper.StepAsync("Increase product quantity to 4 and add to cart", async () =>
            {
                await detailsPage.SetQuantityAsync(4);
                await detailsPage.ClickAddToCartAsync();
                await productsPage.ClickModalViewCartAsync();
            });

            await AllureHelper.StepAsync("Verify that product quantity in the cart is exactly 4", async () =>
            {
                var quantity = await cartPage.GetCartItemQuantityAsync(1);
                Assert.AreEqual(4, quantity, "Product quantity in cart does not match the chosen quantity.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 17: Remove Products From Cart")]
        public async Task RemoveProductFromCart_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);

            await AllureHelper.StepAsync("Navigate to home page and add product to cart", async () =>
            {
                await homePage.NavigateAsync();
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
            });

            await AllureHelper.StepAsync("Remove item from the cart", async () =>
            {
                await cartPage.RemoveFirstItemAsync();
            });

            await AllureHelper.StepAsync("Verify cart is empty", async () =>
            {
                var count = await cartPage.GetCartItemCountAsync();
                Assert.AreEqual(0, count, "Cart is not empty after product removal.");
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 20: Search Products and Verify Cart After Login")]
        public async Task SearchProducts_AndVerifyCart_AfterLogin()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomEmail = RandomDataGenerator.GenerateEmail();
            var username = "CartVerifyUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);
            var password = "Password123!";

            // Create user first to login later
            await AllureHelper.StepAsync("Pre-register a user for testing", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomEmail);
                await signupPage.FillSignupDetailsAsync(
                    password: password,
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Cart",
                    lastName: "User",
                    address1: "123 Cart St",
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90001",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                AccountPendingCleanup = true;
                await homePage.ClickLogoutAsync();
            });

            await AllureHelper.StepAsync("Navigate to Products, search for product and add to cart", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickProductsAsync();
                await productsPage.SearchProductAsync("Blue Top");
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
            });

            await AllureHelper.StepAsync("Verify product is visible in cart", async () =>
            {
                Assert.IsTrue(await cartPage.GetCartItemCountAsync() > 0, "Cart is empty.");
            });

            await AllureHelper.StepAsync("Click 'Signup / Login' and login with pre-registered user", async () =>
            {
                await homePage.ClickLoginSignupAsync();
                await loginPage.LoginAsync(randomEmail, password);
            });

            await AllureHelper.StepAsync("Go back to Cart page and verify product is still in cart", async () =>
            {
                await homePage.ClickCartAsync();
                Assert.IsTrue(await cartPage.GetCartItemCountAsync() > 0, "Cart items were lost after login.");
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
                AccountPendingCleanup = false;
            });
        }

        [Test]
        [Retry(2)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 22: Add to cart from Recommended items")]
        public async Task AddToCart_FromRecommendedItems_ShouldSucceed()
        {
            var homePage = new HomePage(Page);
            var cartPage = new CartPage(Page);

            await AllureHelper.StepAsync("Navigate to home page, scroll to recommended items and add to cart", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.AddFirstRecommendedItemToCartAsync();
            });

            await AllureHelper.StepAsync("Go to cart and verify recommended product is displayed", async () =>
            {
                await homePage.ClickCartAsync();
                Assert.IsTrue(await cartPage.GetCartItemCountAsync() > 0, "Recommended item was not added to cart.");
            });
        }
    }
}