using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Base;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.TestData;
using AutomationExercise.ApiTests.Models.Responses;

namespace AutomationExercise.ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Products")]
    [AllureFeature("Product Catalog Management")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "Integration", "Search")]
    public class SearchProductApiTests : BaseApiTest
    {
        private ProductsApiClient _productsClient = null!;

        [SetUp]
        public void TestSetUp()
        {
            _productsClient = new ProductsApiClient(RequestContext);
        }

        [Test]
        [AllureName("API 5: POST To Search Product")]
        [AllureDescription("Verify that POST request to /api/searchProduct with valid parameter returns matched products.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task SearchProduct_WithValidTerm_ShouldReturnMatchedProducts()
        {
            // Act
            var response = await _productsClient.SearchProductAsync(TestProducts.ValidSearchTerm);

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var searchResponse = JsonHelper.Deserialize<ProductsListResponse>(body);

            Assert.That(searchResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(searchResponse!.ResponseCode, Is.EqualTo(200), "API inner responseCode should be 200");
            Assert.That(searchResponse.Products, Is.Not.Null, "Products list should not be null");
            Assert.That(searchResponse.Products, Is.Not.Empty, "Search results should not be empty");

            // Verify matches
            foreach (var product in searchResponse.Products)
            {
                Assert.That(
                    product.Name.Contains(TestProducts.ValidSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    product.Brand.Contains(TestProducts.ValidSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    product.Category.Category.Contains(TestProducts.ValidSearchTerm, StringComparison.OrdinalIgnoreCase),
                    $"Product Name '{product.Name}', Brand '{product.Brand}', or Category '{product.Category.Category}' should contain term '{TestProducts.ValidSearchTerm}'"
                );
            }
        }

        [Test]
        [AllureName("API 6: POST To Search Product without search_product parameter")]
        [AllureDescription("Verify that POST request to /api/searchProduct without search_product parameter returns 400 response code.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task SearchProduct_WithoutParam_ShouldReturnBadRequest()
        {
            // Act
            var response = await _productsClient.SearchProductWithoutParamAsync();

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(400), "API inner responseCode should be 400 Bad Request");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.SearchParamMissing), "Error message should match expectation");
        }
    }
}
