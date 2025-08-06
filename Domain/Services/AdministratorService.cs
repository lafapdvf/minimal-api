using System.Data.Common;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class AdministratorService : IAdministratorService
{
    private readonly DbContexto _contexto;

    public AdministratorService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrator Create(Administrator administrator)
    {
        _contexto.Administrators.Add(administrator);
        _contexto.SaveChanges();

        return administrator;
    }

    public List<Administrator> GetAll(int? page = 1)
    {
        var query = _contexto.Administrators.AsQueryable();

        int itemsPerPage = 10;

        if (page != null) query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);

        return query.ToList();
    }

    public Administrator? GetById(int id)
    {
        return _contexto.Administrators.Where(v => v.Id == id).FirstOrDefault();
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;

    }
}