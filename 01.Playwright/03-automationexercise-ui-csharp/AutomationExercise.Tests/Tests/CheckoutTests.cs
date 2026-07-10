using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Playwright;
using AutomationExercise.Tests.Base;
using AutomationExercise.Tests.Pages;
using AutomationExercise.Tests.Helpers;
using AutomationExercise.Tests.TestData;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using System.IO;

namespace AutomationExercise.Tests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise")]
    [AllureSubSuite("Checkout & Ordering")]
    [AllureOwner("QA Automation")]
    [AllureTag("UI", "Playwright", "Checkout")]
    public class CheckoutTests : BaseTest
    {
        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 14: Place Order: Register while Checkout")]
        public async Task PlaceOrder_RegisterWhileCheckout()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var checkoutPage = new CheckoutPage(Page);
            var paymentPage = new PaymentPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomUser = RandomDataGenerator.GenerateEmail();
            var username = "CheckoutUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);

            await AllureHelper.StepAsync("Navigate to home page and add product to cart", async () =>
            {
                await homePage.NavigateAsync();
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
            });

            await AllureHelper.StepAsync("Verify cart page is shown and proceed to checkout", async () =>
            {
                var count = await cartPage.GetCartItemCountAsync();
                Assert.IsTrue(count > 0, "Cart is empty.");
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Click 'Register / Login' button in checkout modal", async () =>
            {
                // Wait for the modal Register / Login link to be visible. If it doesn't show up (e.g. if the page reloaded or lagged), click Proceed to Checkout again.
                var registerLoginLocator = Page.Locator("u:has-text('Register / Login')");
                try
                {
                    await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[Checkout UI Warning] Register/Login link did not appear: {ex.Message}. Retrying proceed to checkout...");
                    // Click Proceed to Checkout again
                    await cartPage.ClickProceedToCheckoutAsync();
                    await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                }
                await Page.ClickWithOverloadCheckAsync("u:has-text('Register / Login')");
            });

            await AllureHelper.StepAsync("Register a new user during checkout", async () =>
            {
                await loginPage.SignUpInitAsync(username, randomUser);
                await signupPage.FillSignupDetailsAsync(
                    password: "Password123!",
                    day: "10",
                    month: "June",
                    year: "1992",
                    firstName: "Checkout",
                    lastName: "User",
                    address1: "456 Delivery Rd",
                    country: "United States",
                    state: "California",
                    city: "San Francisco",
                    zipcode: "94101",
                    mobileNumber: "9876543210"
                );
                await signupPage.ClickCreateAccountAsync();
                Assert.IsTrue(await createdPage.IsAccountCreatedVisibleAsync(), "Account was not created successfully.");
                await createdPage.ClickContinueAsync();
            });

            await AllureHelper.StepAsync("Verify logged in status and go back to cart", async () =>
            {
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Not logged in as registered user.");
                await homePage.ClickCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Verify checkout address and place order", async () =>
            {
                var deliveryText = await checkoutPage.GetDeliveryAddressTextAsync();
                Assert.IsTrue(deliveryText.Contains("456 Delivery Rd"), "Address verification failed at checkout.");

                await checkoutPage.EnterCommentAsync("Order comment: Please deliver after 5 PM.");
                await checkoutPage.ClickPlaceOrderAsync();
            });

            await AllureHelper.StepAsync("Enter payment details and submit order", async () =>
            {
                await paymentPage.EnterPaymentDetailsAsync(
                    nameOnCard: "Checkout User",
                    cardNumber: "1234567812345678",
                    cvc: "123",
                    expirationMonth: "12",
                    expirationYear: "2030"
                );
                await paymentPage.ClickPayAndConfirmOrderAsync();
            });

            await AllureHelper.StepAsync("Verify order placement was successful", async () =>
            {
                Assert.IsTrue(await paymentPage.IsOrderSuccessMessageVisibleAsync(), "Order success message is not visible.");
                await paymentPage.ClickContinueAsync();
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                Assert.IsTrue(await deletedPage.IsAccountDeletedVisibleAsync(), "Account deletion failed.");
                await deletedPage.ClickContinueAsync();
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 15: Place Order: Register before Checkout")]
        public async Task PlaceOrder_RegisterBeforeCheckout()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var checkoutPage = new CheckoutPage(Page);
            var paymentPage = new PaymentPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomUser = RandomDataGenerator.GenerateEmail();
            var username = "BeforeCheckoutUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);

            await AllureHelper.StepAsync("Navigate to home page and register user before checkout", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomUser);
                await signupPage.FillSignupDetailsAsync(
                    password: "Password123!",
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Before",
                    lastName: "Checkout",
                    address1: "789 Pre-delivery Rd",
                    country: "Canada",
                    state: "Ontario",
                    city: "Toronto",
                    zipcode: "M5V 2T6",
                    mobileNumber: "1112223333"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Login state is incorrect.");
            });

            await AllureHelper.StepAsync("Add product to cart and proceed to checkout", async () =>
            {
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Place order and pay", async () =>
            {
                await checkoutPage.EnterCommentAsync("Pre-registered order comment.");
                await checkoutPage.ClickPlaceOrderAsync();

                await paymentPage.EnterPaymentDetailsAsync(
                    nameOnCard: "Before Checkout User",
                    cardNumber: "4321432143214321",
                    cvc: "456",
                    expirationMonth: "08",
                    expirationYear: "2032"
                );
                await paymentPage.ClickPayAndConfirmOrderAsync();
                Assert.IsTrue(await paymentPage.IsOrderSuccessMessageVisibleAsync(), "Order failed.");
                await paymentPage.ClickContinueAsync();
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 16: Place Order: Login before Checkout")]
        public async Task PlaceOrder_LoginBeforeCheckout()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var checkoutPage = new CheckoutPage(Page);
            var paymentPage = new PaymentPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomEmail = RandomDataGenerator.GenerateEmail();
            var username = "LoginCheckoutUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);
            var password = "Password123!";

            await AllureHelper.StepAsync("Navigate to home page and register user", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomEmail);
                await signupPage.FillSignupDetailsAsync(
                    password: password,
                    day: "1",
                    month: "January",
                    year: "1990",
                    firstName: "Login",
                    lastName: "Checkout",
                    address1: "123 LoginCheckout St",
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90001",
                    mobileNumber: "1234567890"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                
                // Logout to clear the session so we can test Login before Checkout
                await homePage.ClickLogoutAsync();
            });

            await AllureHelper.StepAsync("Log in before checkout", async () =>
            {
                await homePage.ClickLoginSignupAsync();
                await loginPage.LoginAsync(randomEmail, password);
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Failed to log in.");
            });

            await AllureHelper.StepAsync("Add product to cart and proceed to checkout", async () =>
            {
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Place order and pay", async () =>
            {
                await checkoutPage.EnterCommentAsync("Log in before checkout order comment.");
                await checkoutPage.ClickPlaceOrderAsync();

                await paymentPage.EnterPaymentDetailsAsync(
                    nameOnCard: username,
                    cardNumber: "1111222233334444",
                    cvc: "999",
                    expirationMonth: "01",
                    expirationYear: "2029"
                );
                await paymentPage.ClickPayAndConfirmOrderAsync();
                Assert.IsTrue(await paymentPage.IsOrderSuccessMessageVisibleAsync(), "Order failed.");
                await paymentPage.ClickContinueAsync();
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Test Case 23: Verify address details in checkout page")]
        public async Task VerifyAddressDetails_InCheckoutPage()
        {
            var homePage = new HomePage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var checkoutPage = new CheckoutPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomUser = RandomDataGenerator.GenerateEmail();
            var username = "AddressUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);

            var expectedAddress = "100 Address Lane";
            var expectedCityStateZip = "Los Angeles California 90002";

            await AllureHelper.StepAsync("Register a new user with specific address", async () =>
            {
                await homePage.NavigateAsync();
                await homePage.ClickLoginSignupAsync();
                await loginPage.SignUpInitAsync(username, randomUser);
                await signupPage.FillSignupDetailsAsync(
                    password: "Password123!",
                    day: "5",
                    month: "May",
                    year: "1995",
                    firstName: "Address",
                    lastName: "Verifier",
                    address1: expectedAddress,
                    country: "United States",
                    state: "California",
                    city: "Los Angeles",
                    zipcode: "90002",
                    mobileNumber: "9998887777"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                Assert.IsTrue(await homePage.IsLoggedInUserVisibleAsync(username), "Incorrect login state.");
            });

            await AllureHelper.StepAsync("Add product to cart and proceed to checkout", async () =>
            {
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Verify delivery and billing addresses are identical and match registration", async () =>
            {
                var deliveryAddress = await checkoutPage.GetDeliveryAddressTextAsync();
                var billingAddress = await checkoutPage.GetBillingAddressTextAsync();

                Assert.IsTrue(deliveryAddress.Contains(expectedAddress), "Delivery address does not match expected address.");
                Assert.IsTrue(deliveryAddress.Contains(expectedCityStateZip), "Delivery city/state/zip verification failed.");
                
                Assert.IsTrue(billingAddress.Contains(expectedAddress), "Billing address does not match expected address.");
                Assert.IsTrue(billingAddress.Contains(expectedCityStateZip), "Billing city/state/zip verification failed.");
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }

        [Test]
        [Retry(3)]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Test Case 24: Download Invoice after purchase order")]
        public async Task DownloadInvoice_AfterOrderPurchase()
        {
            var homePage = new HomePage(Page);
            var productsPage = new ProductsPage(Page);
            var cartPage = new CartPage(Page);
            var loginPage = new LoginPage(Page);
            var signupPage = new SignupPage(Page);
            var createdPage = new AccountCreatedPage(Page);
            var checkoutPage = new CheckoutPage(Page);
            var paymentPage = new PaymentPage(Page);
            var deletedPage = new AccountDeletedPage(Page);

            var randomUser = RandomDataGenerator.GenerateEmail();
            var username = "InvoiceUser_" + System.Guid.NewGuid().ToString().Substring(0, 5);

            await AllureHelper.StepAsync("Register a user and proceed to checkout with items", async () =>
            {
                await homePage.NavigateAsync();
                await productsPage.AddFirstProductToCartAsync();
                await productsPage.ClickModalViewCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();

                // Wait for the modal Register / Login link to be visible. If it doesn't show up, click Proceed to Checkout again.
                var registerLoginLocator = Page.Locator("u:has-text('Register / Login')");
                try
                {
                    await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[Checkout UI Warning] Register/Login link did not appear: {ex.Message}. Retrying proceed to checkout...");
                    await cartPage.ClickProceedToCheckoutAsync();
                    await registerLoginLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                }
                await Page.ClickWithOverloadCheckAsync("u:has-text('Register / Login')");
                await loginPage.SignUpInitAsync(username, randomUser);
                await signupPage.FillSignupDetailsAsync(
                    password: "Password123!",
                    day: "20",
                    month: "December",
                    year: "1988",
                    firstName: "Invoice",
                    lastName: "Downloader",
                    address1: "123 Invoice St",
                    country: "United States",
                    state: "Washington",
                    city: "Seattle",
                    zipcode: "98101",
                    mobileNumber: "2223334444"
                );
                await signupPage.ClickCreateAccountAsync();
                await createdPage.ClickContinueAsync();
                await homePage.ClickCartAsync();
                await cartPage.ClickProceedToCheckoutAsync();
            });

            await AllureHelper.StepAsync("Place order and pay", async () =>
            {
                await checkoutPage.ClickPlaceOrderAsync();
                await paymentPage.EnterPaymentDetailsAsync(
                    nameOnCard: "Invoice User",
                    cardNumber: "1111222233334444",
                    cvc: "123",
                    expirationMonth: "10",
                    expirationYear: "2031"
                );
                await paymentPage.ClickPayAndConfirmOrderAsync();
                Assert.IsTrue(await paymentPage.IsOrderSuccessMessageVisibleAsync(), "Failed to place order.");
            });

            await AllureHelper.StepAsync("Download Invoice and verify file download", async () =>
            {
                var downloadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Downloads");
                var downloadedFilePath = await paymentPage.ClickDownloadInvoiceAsync(downloadDir);

                Assert.IsTrue(File.Exists(downloadedFilePath), "Downloaded invoice file does not exist on disk.");
                Assert.IsTrue(new FileInfo(downloadedFilePath).Length > 0, "Downloaded invoice file is empty.");

                // Add to allure report as an attachment
                AllureHelper.AddTextAttachment("Downloaded Invoice Path", downloadedFilePath);
                
                // Clean up file
                File.Delete(downloadedFilePath);

                await paymentPage.ClickContinueAsync();
            });

            await AllureHelper.StepAsync("Clean up: Delete the user account", async () =>
            {
                await homePage.ClickDeleteAccountAsync();
                await deletedPage.ClickContinueAsync();
            });
        }
    }
}