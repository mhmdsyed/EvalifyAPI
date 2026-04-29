using Evalify.Application.Common.Interfaces;

namespace Evalify.Infrastructure.Services;

/// <summary>
/// Mock AI service - replace with real HTTP call when AI endpoint is ready.
/// </summary>
public sealed class AiService : IAiService
{
    public Task<AiEvaluationResult> EvaluateAsync(
        Stream croppedImageStream,
        string modelAnswer,
        double maxMark,
        CancellationToken ct)
    {
        var random = new Random();
        var grade = Math.Round(random.NextDouble() * maxMark, 2);

        return Task.FromResult(new AiEvaluationResult
        {
            Success = true,
            Grade = grade,
            ExtractedText = "Mock extracted text - AI endpoint not connected yet."
        });
    }
}
