using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDTO);
}