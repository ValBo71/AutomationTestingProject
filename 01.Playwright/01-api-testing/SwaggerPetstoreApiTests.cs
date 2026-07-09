using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace ApiTesting;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SwaggerPetstoreApiTests : PlaywrightTest
{
    private IAPIRequestContext _request = null!;
    
    // Променливи от environment / collection
    private readonly string _baseUrl = "https://petstore.swagger.io";
    
    // Тестови данни, генериращи се динамично като в Postman
    private string _nameUser = $"User_{Guid.NewGuid().ToString()[..8]}";
    private string _firstName = "TestFirst";
    private string _lastName = "TestLast";
    private string _emailUser = $"test_{Guid.NewGuid().ToString()[..5]}@example.com";
    private string _pass = "InitialPass123";
    private string _phone = "1234567890";
    
    [SetUp]
    public async Task SetUp()
    {
        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _baseUrl,
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_request != null)
        {
            await _request.DisposeAsync();
        }
    }

    [Test]
    public async Task UserLifecycleTest()
    {
        // 1. Create User
        var createPayload = new Dictionary<string, string>
        {
            { "username", _nameUser },
            { "firstName", _firstName },
            { "lastName", _lastName },
            { "email", _emailUser },
            { "password", _pass },
            { "phone", _phone }
        };

        var createResponse = await _request.PostAsync("/v2/user", new APIRequestContextOptions
        {
            DataObject = createPayload
        });
        
        Assert.That(createResponse.Status, Is.EqualTo(200), "Create User статус кодът трябва да е 200");
        
        var createJson = await createResponse.JsonAsync();
        string userId = createJson?.GetProperty("message").GetString() ?? "";

        // 2. Get User
        var getResponse = await _request.GetAsync($"/v2/user/{_nameUser}");
        Assert.That(getResponse.Status, Is.EqualTo(200), "Get User статус кодът трябва да е 200");
        
        var getJson = await getResponse.JsonAsync();
        Assert.That(getJson?.GetProperty("username").GetString(), Is.EqualTo(_nameUser), "Потребителското име съвпада с подаденото");
        Assert.That(getJson?.GetProperty("firstName").GetString(), Is.EqualTo(_firstName), "Първото име съвпада с подаденото");
        Assert.That(getJson?.GetProperty("email").GetString(), Is.EqualTo(_emailUser), "Имейлът съвпада с подадения");

        // 3. Update User
        _pass = "UpdatedPass123";
        _phone = "0987654321";
        
        var updatePayload = new Dictionary<string, object>
        {
            { "id", long.Parse(userId) },
            { "username", _nameUser },
            { "firstName", _firstName },
            { "lastName", _lastName },
            { "email", _emailUser },
            { "password", _pass },
            { "phone", _phone },
            { "userStatus", 0 }
        };

        var updateResponse = await _request.PutAsync($"/v2/user/{_nameUser}", new APIRequestContextOptions
        {
            DataObject = updatePayload
        });
        Assert.That(updateResponse.Status, Is.EqualTo(200), "Update User статус кодът трябва да е 200");
        
        // Петстор API-то понякога иска време за опресняване
        await Task.Delay(1000);

        // 4. Get Updated User
        var getUpdatedResponse = await _request.GetAsync($"/v2/user/{_nameUser}");
        Assert.That(getUpdatedResponse.Status, Is.EqualTo(200), "Get Updated User статус кодът трябва да е 200");
        
        var getUpdatedJson = await getUpdatedResponse.JsonAsync();
        Assert.That(getUpdatedJson?.GetProperty("phone").GetString(), Is.EqualTo(_phone), "Телефонният номер съвпада с обновения");
        Assert.That(getUpdatedJson?.GetProperty("password").GetString(), Is.EqualTo(_pass), "Паролата съвпада с обновената");

        // 5. Login User
        var loginResponse = await _request.GetAsync($"/v2/user/login?username={_nameUser}&password={_pass}");
        Assert.That(loginResponse.Status, Is.EqualTo(200), "Login статус кодът трябва да е 200");

        // 6. Logout User
        var logoutResponse = await _request.GetAsync("/v2/user/logout");
        Assert.That(logoutResponse.Status, Is.EqualTo(200), "Logout статус кодът трябва да е 200");
        
        var logoutJson = await logoutResponse.JsonAsync();
        Assert.That(logoutJson?.GetProperty("message").GetString(), Is.EqualTo("ok"), "Съобщението при изход трябва да е 'ok'");

        // 7. Delete User
        var deleteResponse = await _request.DeleteAsync($"/v2/user/{_nameUser}");
        Assert.That(deleteResponse.Status, Is.EqualTo(200), "Delete статус кодът трябва да е 200");
        
        var deleteJson = await deleteResponse.JsonAsync();
        Assert.That(deleteJson?.GetProperty("message").GetString(), Is.EqualTo(_nameUser), "Съобщението при изтриване трябва да съдържа потребителското име");
    }
}
