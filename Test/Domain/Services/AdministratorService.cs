using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;

namespace Test.Domanin.Entities;

[TestClass]
public class AdministratorServiceTest
{
    private DbContexto CreateTestContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void AdministratorSaveTest()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators;");

        var adm = new Administrator();
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Profile = "Adm";

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Create(adm);

        // Assert
        Assert.AreEqual(1, administratorService.GetAll().Count());
    }

    [TestMethod]
    public void AdministratorGetByIdTest()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators;");

        var adm = new Administrator();
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Profile = "Adm";

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Create(adm);
        var dbAdm = administratorService.GetById(adm.Id);

        // Assert
        Assert.AreEqual(1, dbAdm?.Id);
    }

    // TODO: Implement more tests for the AdministratorService methods
}