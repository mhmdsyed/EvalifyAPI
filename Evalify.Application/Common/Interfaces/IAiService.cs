namespace Evalify.Application.Common.Interfaces;

public interface IAiService
{
    Task<AiEvaluationResult> EvaluateAsync(
        Stream croppedImageStream,
        string modelAnswer,
        double maxMark,
        CancellationToken ct);
}

public sealed class AiEvaluationResult
{
    public bool Success { get; init; }
    public double Grade { get; init; }
    public string? ExtractedText { get; init; }
}
