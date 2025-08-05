using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle> GetAll(int page = 1, string? model = null, string? brand = null);
    Vehicle? GetById(int id);
    void Create(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vehicle);
}