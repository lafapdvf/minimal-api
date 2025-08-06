using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ViewModels;
using MinimalApi.Domain.Services;
using MinimalApi.Domain.Enums;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) is not null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorViewModel>();
    var administrators = administratorService.GetAll(page);
    foreach (var adm in administrators)
    {
        adms.Add(new AdministratorViewModel
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }
    return Results.Ok(adms);
}).WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);
    if (administrator is null) return Results.NotFound();
    return Results.Ok(new AdministratorViewModel
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email)) validation.Messages.Add("O e-mail não pode ser vazio.");
    if (string.IsNullOrEmpty(administratorDTO.Password)) validation.Messages.Add("A senha não pode ser vazia.");
    if (administratorDTO.Profile == null) validation.Messages.Add("O perfil não pode ser vazio.");

    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile?.ToString() ?? Profile.Editor.ToString()
    };
    administratorService.Create(administrator);

    return Results.Created($"/administrators/{administrator.Id}", new AdministratorViewModel
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");
#endregion

#region Vehicles
ValidationErrors validateDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Model)) validation.Messages.Add("O modelo do veículo não pode ser vazio.");


    if (string.IsNullOrEmpty(vehicleDTO.Brand)) validation.Messages.Add("A marca do veículo não pode ficar em branco.");


    if (vehicleDTO.Year < 1950) validation.Messages.Add("O ano do veículo deve ser maior ou igual a 1950.");

    return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validation = validateDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var vehicle = new Vehicle
    {
        Model = vehicleDTO.Model,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };
    vehicleService.Create(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);
    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle is null) return Results.NotFound();
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle is null) return Results.NotFound();

    var validation = validateDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    vehicle.Model = vehicleDTO.Model;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle is null) return Results.NotFound();

    vehicleService.Delete(vehicle);
    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

