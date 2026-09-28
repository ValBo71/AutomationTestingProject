using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Base;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.Models.Responses;

namespace AutomationExercise.ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Products")]
    [AllureFeature("Product Catalog Management")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "Integration", "Products")]
    public class ProductsApiTests : BaseApiTest
    {
        private ProductsApiClient _productsClient = null!;

        [SetUp]
        public void TestSetUp()
        {
            _productsClient = new ProductsApiClient(RequestContext);
        }

        [Test]
        [AllureName("API 1: Get All Products List")]
        [AllureDescription("Verify that GET request to /api/productsList returns all products with required properties.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetAllProductsList_ShouldReturnAllProducts()
        {
            // Act
            var response = await _productsClient.GetAllProductsAsync();

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var productsResponse = JsonHelper.Deserialize<ProductsListResponse>(body);

            Assert.That(productsResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(productsResponse!.ResponseCode, Is.EqualTo(200), "API inner responseCode should be 200");
            Assert.That(productsResponse.Products, Is.Not.Null, "Products list should not be null");
            Assert.That(productsResponse.Products, Is.Not.Empty, "Products list should not be empty");

            // Every product, not just the first - the description promises "all products".
            // Is.Not.Empty rather than Is.Not.Null.Or.Empty: the latter reads as "not (null or empty)"
            // but NUnit parses it as "not null, or empty", which an empty string satisfies. The models
            // default missing fields to "", so that version passed even when a field was absent.
            Assert.That(productsResponse.Products.Select(p => p.Id), Is.Unique, "Product IDs should be unique");
            Assert.Multiple(() =>
            {
                foreach (var product in productsResponse.Products)
                {
                    var which = $"product {product.Id}";
                    Assert.That(product.Id, Is.GreaterThan(0), $"{which}: ID should be greater than 0");
                    Assert.That(product.Name, Is.Not.Empty, $"{which}: Name should not be empty");
                    Assert.That(product.Price, Does.Match(@"^Rs\. \d+$"), $"{which}: Price should look like 'Rs. 500'");
                    Assert.That(product.Brand, Is.Not.Empty, $"{which}: Brand should not be empty");
                    Assert.That(product.Category.Category, Is.Not.Empty, $"{which}: Category should not be empty");
                    Assert.That(product.Category.Usertype.Usertype, Is.Not.Empty, $"{which}: Usertype should not be empty");
                }
            });
        }

        [Test]
        [AllureName("API 2: POST To All Products List")]
        [AllureDescription("Verify that POST request to /api/productsList is not supported and returns 405 response code.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PostToProductsList_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await _productsClient.PostToProductsListAsync();

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(405), "API inner responseCode should be 405 Method Not Supported");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.MethodNotSupported), "Message should match expectation");
        }
    }
}
