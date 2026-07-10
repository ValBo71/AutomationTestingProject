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
    [AllureSubSuite("Products")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "RestSharp", "Products")]
    public class ProductsApiTests : BaseApiTest
    {
        [Test]
        [Description("API 1: GET All Products List")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetAllProducts_ShouldReturnProductsList()
        {
            // Act
            var response = await ProductsClient.GetAllProductsAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var body = ResponseHelper.Deserialize<ProductsListResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(200));
            Assert.That(body.Products, Is.Not.Null.And.Not.Empty);

            foreach (var product in body.Products)
            {
                Assert.That(product.Id, Is.GreaterThan(0));
                Assert.That(product.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(product.Price, Is.Not.Null.And.Not.Empty);
                Assert.That(product.Brand, Is.Not.Null.And.Not.Empty);
                Assert.That(product.Category, Is.Not.Null);
            }
        }

        [Test]
        [Description("API 2: POST To All Products List")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PostToProductsList_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await ProductsClient.PostToProductsListAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(405));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.MethodNotSupported));
        }
    }
}
