namespace Ecomm.Catalog.Models.ViewModels;

public record CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}