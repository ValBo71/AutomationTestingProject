using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace ApiTesting;

/// <summary>
/// Deliberately raw: one test, inline requests, no clients or models - the "before" half of the
/// before/after comparison with project 04. Raw is about structure, not about what the test proves.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SwaggerPetstoreApiTests : PlaywrightTest
{
    private IAPIRequestContext _request = null!;

    // Hard-coded here - the C# counterpart of {{baseURL}} in the Postman environment.
    private readonly string _baseUrl = "https://petstore.swagger.io";

    // Test data. Only the username and email are generated (unique per run, so the test cannot collide
    // with another visitor's user on the shared Petstore); the rest are fixed values.
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
        var userDeleted = false;
        try
        {
            // 1. Create User - inside the try, so a failed assertion here still reaches the cleanup below.
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
            Assert.That(createJson.HasValue && createJson.Value.TryGetProperty("message", out _), Is.True,
                "Create User отговорът трябва да съдържа поле 'message' с ID-то на потребителя");
            var userId = long.Parse(createJson!.Value.GetProperty("message").GetString() ?? "");

            // 2. Get User - polled, because the public Petstore does not always return a user on the very
            // next request after it was written.
            var getJson = await WaitForUserAsync(user => user.GetProperty("id").GetInt64() == userId,
                "the created user should be readable");
            Assert.That(getJson.GetProperty("username").GetString(), Is.EqualTo(_nameUser), "Потребителското име съвпада с подаденото");
            Assert.That(getJson.GetProperty("firstName").GetString(), Is.EqualTo(_firstName), "Първото име съвпада с подаденото");
            Assert.That(getJson.GetProperty("email").GetString(), Is.EqualTo(_emailUser), "Имейлът съвпада с подадения");

            // 3. Update User. The id has to be in the body: Petstore silently ignores a PUT without it,
            // keeping the old values (checked against the live API).
            _pass = "UpdatedPass123";
            _phone = "0987654321";

            var updatePayload = new Dictionary<string, object>
            {
                { "id", userId },
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

            // 4. Get Updated User - polled for the new phone, instead of a fixed one-second sleep.
            var getUpdatedJson = await WaitForUserAsync(user => user.GetProperty("phone").GetString() == _phone,
                "the update should be readable");
            Assert.That(getUpdatedJson.GetProperty("phone").GetString(), Is.EqualTo(_phone), "Телефонният номер съвпада с обновения");
            Assert.That(getUpdatedJson.GetProperty("password").GetString(), Is.EqualTo(_pass), "Паролата съвпада с обновената");

            // 5. Login User. The public Petstore does not check credentials - any username and password get
            // a session - so this step cannot prove that the updated password works. Step 4 is what proves
            // the new password was stored; here only the shape of the answer is checked.
            var loginResponse = await _request.GetAsync($"/v2/user/login?username={_nameUser}&password={_pass}");
            Assert.That(loginResponse.Status, Is.EqualTo(200), "Login статус кодът трябва да е 200");
            var loginJson = await loginResponse.JsonAsync();
            Assert.That(loginJson?.GetProperty("message").GetString(), Does.StartWith("logged in user session:"),
                "Login трябва да върне сесия");

            // 6. Logout User
            var logoutResponse = await _request.GetAsync("/v2/user/logout");
            Assert.That(logoutResponse.Status, Is.EqualTo(200), "Logout статус кодът трябва да е 200");

            var logoutJson = await logoutResponse.JsonAsync();
            Assert.That(logoutJson?.GetProperty("message").GetString(), Is.EqualTo("ok"), "Съобщението при изход трябва да е 'ok'");

            // 7. Delete User, then prove it is gone: the delete's own answer only echoes the username.
            var deleteResponse = await _request.DeleteAsync($"/v2/user/{_nameUser}");
            Assert.That(deleteResponse.Status, Is.EqualTo(200), "Delete статус кодът трябва да е 200");

            var deleteJson = await deleteResponse.JsonAsync();
            Assert.That(deleteJson?.GetProperty("message").GetString(), Is.EqualTo(_nameUser), "Съобщението при изтриване трябва да съдържа потребителското име");

            await WaitForStatusAsync(404, "the deleted user should no longer be found");
            userDeleted = true;
        }
        finally
        {
            // Safety net: whatever failed above - including the create assertions - the user is removed from
            // the shared petstore.swagger.io sandbox instead of being left behind. A 404 means it never
            // existed or is already gone.
            if (!userDeleted)
            {
                try
                {
                    await _request.DeleteAsync($"/v2/user/{_nameUser}");
                }
                catch
                {
                    // Best effort only - the original assertion failure above is what should surface.
                }
            }
        }
    }

    /// <summary>GETs the user until <paramref name="isReady"/> holds, for up to about five seconds.</summary>
    private async Task<JsonElement> WaitForUserAsync(Func<JsonElement, bool> isReady, string what)
    {
        for (var attempt = 1; ; attempt++)
        {
            var response = await _request.GetAsync($"/v2/user/{_nameUser}");
            if (response.Status == 200)
            {
                var json = await response.JsonAsync();
                if (json.HasValue && isReady(json.Value))
                {
                    return json.Value;
                }
            }
            if (attempt == 10)
            {
                Assert.Fail($"Timed out waiting: {what} (last GET answered {response.Status}).");
            }
            await Task.Delay(500);
        }
    }

    /// <summary>GETs the user until the answer has the expected status, for up to about five seconds.</summary>
    private async Task WaitForStatusAsync(int expected, string what)
    {
        for (var attempt = 1; ; attempt++)
        {
            var status = (await _request.GetAsync($"/v2/user/{_nameUser}")).Status;
            if (status == expected)
            {
                return;
            }
            if (attempt == 10)
            {
                Assert.Fail($"Timed out waiting: {what} (last GET answered {status}).");
            }
            await Task.Delay(500);
        }
    }
}
