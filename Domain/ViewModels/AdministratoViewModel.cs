using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.ViewModels;

public record AdministratorViewModel
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
}