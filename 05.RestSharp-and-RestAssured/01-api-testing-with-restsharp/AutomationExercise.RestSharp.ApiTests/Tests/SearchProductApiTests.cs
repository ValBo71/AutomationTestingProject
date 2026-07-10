using System.Net;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using AutomationExercise.RestSharp.ApiTests.Base;
using AutomationExercise.RestSharp.ApiTests.Constants;
using AutomationExercise.RestSharp.ApiTests.Helpers;
using AutomationExercise.RestSharp.ApiTests.Models.Responses;
using NUnit.Framework;

namespace AutomationExercise.RestSharp.ApiTests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Products Search")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "RestSharp", "Products", "Search")]
    public class SearchProductApiTests : BaseApiTest
    {
        [Test]
        [Description("API 5: POST To Search Product")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task SearchProduct_WithValidProduct_ShouldReturnProducts()
        {
            // Arrange
            const string searchWord = "tshirt";

            // Act
            var response = await ProductsClient.SearchProductAsync(searchWord);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ProductsListResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(200));
            Assert.That(body.Products, Is.Not.Null.And.Not.Empty);

            foreach (var product in body.Products)
            {
                Assert.That(product.Name.ToLower(), Does.Contain(searchWord).Or.Contain("shirt").Or.Contain("polo"));
            }
        }

        [Test]
        [Description("API 6: POST To Search Product without search_product parameter")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task SearchProduct_WithoutSearchProductParameter_ShouldReturnBadRequest()
        {
            // Act
            var response = await ProductsClient.SearchProductWithoutParameterAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(400));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.SearchProductMissing));
        }
    }
}
