using Evalify.Domain.Common.Results;

namespace Evalify.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error InvalidCredentials =>
        Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

    public static Error EmailAlreadyExists =>
        Error.Conflict("Auth.EmailAlreadyExists", "Email is already registered.");

    public static Error TokenGenerationFailed =>
        Error.Failure("Auth.TokenGenerationFailed", "Failed to generate token.");

    public static Error TemplateNotFound =>
        Error.NotFound("Template.NotFound", "Template does not exist.");

    public static Error TemplateNotOwnedByUser =>
        Error.Forbidden("Template.NotOwned", "You do not own this template.");

    public static Error QuestionsNotFound =>
        Error.NotFound("Questions.NotFound", "No questions found for this template.");

    public static Error PaperNotFound =>
        Error.NotFound("Paper.NotFound", "Student paper does not exist.");

    public static Error PaperAlreadyExists =>
        Error.Conflict("Paper.AlreadyExists", "A paper with this student code already exists for this template.");

    public static Error AnswerNotFound =>
        Error.NotFound("Answer.NotFound", "Student answer does not exist.");

    public static Error GradeExceedsMaxMark =>
        Error.Validation("Answer.GradeExceedsMaxMark", "Grade cannot exceed the question maximum mark.");

    public static Error AiEvaluationFailed =>
        Error.Failure("AI.EvaluationFailed", "AI service failed to evaluate the answer.");
}
