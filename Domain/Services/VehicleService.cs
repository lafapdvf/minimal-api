using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DbContexto _contexto;

    public VehicleService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Vehicle> GetAll(int page = 1, string? model = null, string? brand = null)
    {
        var query = _contexto.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(model))
        {
            query = query.Where(v => EF.Functions.Like(v.Model.ToLower(), $"%{model.ToLower()}%"));
        }

        int itemsPerPage = 10;

        query = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

        return query.ToList();
    }

    public Vehicle? GetById(int id)
    {
        return _contexto.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Create(Vehicle vehicle)
    {
        _contexto.Vehicles.Add(vehicle);
        _contexto.SaveChanges();
    }

    public void Update(Vehicle vehicle)
    {
        _contexto.Vehicles.Update(vehicle);
        _contexto.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        _contexto.Vehicles.Remove(vehicle);
        _contexto.SaveChanges();
    }
}