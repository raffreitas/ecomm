using System.ComponentModel.DataAnnotations;

namespace Ecomm.Catalog.Api.Models.InputModel;

public record CreateCategoryInputModel
{
    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
}