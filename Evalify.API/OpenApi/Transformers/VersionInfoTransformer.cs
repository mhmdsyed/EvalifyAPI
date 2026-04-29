using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Evalify.API.OpenApi.Transformers;

internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"Evalify API {version}";
        document.Info.Description = "AI-Powered Handwritten Answer Evaluation Platform";

        return Task.CompletedTask;
    }
}
