using System.ComponentModel.DataAnnotations;

namespace Ecomm.Catalog.Api.Models.InputModel;

public record CreateProductInputModel
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public decimal Price { get; set; }
    [Required] public string ImageUrl { get; set; } = string.Empty;
    [Required] public Guid CategoryId { get; set; }
}