using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ViewModels;
using MinimalApi.Domain.Services;
using MinimalApi.Domain.Enums;
using MinimalApi.Infrastructure.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token here."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minimal API",
        Version = "v1"
    });
});

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrators
string GenerateJwtToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Profile", administrator.Profile),
        new Claim(ClaimTypes.Role, administrator.Profile)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);
    if (adm is not null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new LoggedAdmViewModel
        {
            Email = adm.Email,
            Profile = adm.Profile,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrators");

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
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administrators");

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
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administrators");

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
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administrators");
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
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);
    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle is null) return Results.NotFound();
    return Results.Ok(vehicle);
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Vehicles");

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
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle is null) return Results.NotFound();

    vehicleService.Delete(vehicle);
    return Results.NoContent();
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion

