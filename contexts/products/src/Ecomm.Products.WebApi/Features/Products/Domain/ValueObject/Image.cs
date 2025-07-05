using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Products.Domain.ValueObject;

public sealed record Image : IValueObject
{
    public string Url { get; init; }
    public string AltText { get; init; }

    private Image(string url, string altText)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be null or empty.", nameof(url));
        if (string.IsNullOrWhiteSpace(altText))
            throw new ArgumentException("Alt text cannot be null or empty.", nameof(altText));
        Url = url;
        AltText = altText;
    }

    public static Image Create(string url, string altText) => new(url, altText);
}
