using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

namespace Ecomm.Orders.Api.Extensions;

public static class OpenApiExtensions
{
    public static void MapApiReference(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    public static void AddApiReference(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
    }

    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authSchemes.Any(scheme => scheme.Name == JwtBearerDefaults.AuthenticationScheme))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    [JwtBearerDefaults.AuthenticationScheme] = new()
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;
            }
        }
    }
}