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
    [AllureSubSuite("Brands")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "RestSharp", "Brands")]
    public class BrandsApiTests : BaseApiTest
    {
        [Test]
        [Description("API 3: GET All Brands List")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetAllBrands_ShouldReturnBrandsList()
        {
            // Act
            var response = await BrandsClient.GetAllBrandsAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<BrandsListResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(200));
            Assert.That(body.Brands, Is.Not.Null.And.Not.Empty);

            foreach (var brand in body.Brands)
            {
                Assert.That(brand.Id, Is.GreaterThan(0));
                Assert.That(brand.BrandName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        [Description("API 4: PUT To All Brands List")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PutToBrandsList_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await BrandsClient.PutToBrandsListAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(405));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.MethodNotSupported));
        }
    }
}
