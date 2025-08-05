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

    public Administrator? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;

    }
}