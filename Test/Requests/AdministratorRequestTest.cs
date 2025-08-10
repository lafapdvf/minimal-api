using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ViewModels;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public sealed class AdministratorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administrators/login", content);

        // Assert
        Assert.AreEqual(200, (int)response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdm = JsonSerializer.Deserialize<LoggedAdmViewModel>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdm?.Email ?? "");
        Assert.IsNotNull(loggedAdm?.Profile ?? "");
        Assert.IsNotNull(loggedAdm?.Token ?? "");

        // TODO: The remaining tests.
    }
}