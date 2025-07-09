using System.ComponentModel.DataAnnotations;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.AI.Settings;

public sealed record SemanticKernelSettings
{
    public const string SectionName = "SemanticKernel";

    [Required, MinLength(1)]
    public required string ApiKey { get; init; }

    [Required, MinLength(1)]
    public required string ModelName { get; init; }
}
