using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks;

public class AdministratorsServiceMock : IAdministratorService
{
    private static List<Administrator> administrators = new List<Administrator>()
    {
        new Administrator{
            Id = 1,
            Email = "adm@teste.com",
            Password = "123456",
            Profile = "Adm"
        },
        new Administrator{
            Id = 2,
            Email = "editor@teste.com",
            Password = "123456",
            Profile = "Editor"
        }
    };

    public Administrator Create(Administrator administrator)
    {
        administrator.Id = administrators.Count() + 1;
        administrators.Add(administrator);

        return administrator;
    }

    public List<Administrator> GetAll(int? page)
    {
        return administrators;
    }

    public Administrator? GetById(int id)
    {
        return administrators.Find(a => a.Id == id);
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }
}