namespace Ecomm.Catalog.Api.Models.ViewModels;

public record CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}