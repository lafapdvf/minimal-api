using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [MaxLength(150)]
    public string Model { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = default!;

    [Required]
    public int Year { get; set; } = default!;
}