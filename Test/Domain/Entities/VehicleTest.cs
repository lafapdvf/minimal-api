using MinimalApi.Domain.Entities;

namespace Test.Domanin.Entities;

[TestClass]
public sealed class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var vehicle = new Vehicle();

        // Act
        vehicle.Id = 1;
        vehicle.Model = "teste";
        vehicle.Brand = "teste";
        vehicle.Year = 1950;

        // Assert
        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("teste", vehicle.Model);
        Assert.AreEqual("teste", vehicle.Brand);
        Assert.AreEqual(1950, vehicle.Year);
    }
}