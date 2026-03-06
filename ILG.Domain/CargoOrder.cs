using System.ComponentModel.DataAnnotations;

namespace ILG.Domain.Models;

public record CargoOrder
{
    [Key] // Define como Chave Primária
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public double Weight { get; init; }
    public DateTime CreatedAt { get; init; }
}