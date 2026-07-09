using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
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
    [AllureSubSuite("Brands")]
    [AllureFeature("Brand Directory Management")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "Integration", "Brands")]
    public class BrandsApiTests : BaseApiTest
    {
        private BrandsApiClient _brandsClient = null!;

        [SetUp]
        public void TestSetUp()
        {
            _brandsClient = new BrandsApiClient(RequestContext);
        }

        [Test]
        [AllureName("API 3: Get All Brands List")]
        [AllureDescription("Verify that GET request to /api/brandsList returns all brands with required properties.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetAllBrandsList_ShouldReturnAllBrands()
        {
            // Act
            var response = await _brandsClient.GetAllBrandsAsync();

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var brandsResponse = JsonHelper.Deserialize<BrandsListResponse>(body);

            Assert.That(brandsResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(brandsResponse!.ResponseCode, Is.EqualTo(200), "API inner responseCode should be 200");
            Assert.That(brandsResponse.Brands, Is.Not.Null, "Brands list should not be null");
            Assert.That(brandsResponse.Brands, Is.Not.Empty, "Brands list should not be empty");

            // Verify a brand schema
            var brand = brandsResponse.Brands[0];
            Assert.Multiple(() =>
            {
                Assert.That(brand.Id, Is.GreaterThan(0), "Brand ID should be greater than 0");
                Assert.That(brand.Brand, Is.Not.Null.Or.Empty, "Brand Name should not be empty");
            });
        }

        [Test]
        [AllureName("API 4: PUT To All Brands List")]
        [AllureDescription("Verify that PUT request to /api/brandsList is not supported and returns 405 response code.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task PutToBrandsList_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await _brandsClient.PutToBrandsListAsync();

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
