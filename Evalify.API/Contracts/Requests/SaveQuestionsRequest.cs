namespace Evalify.API.Contracts.Requests;

public sealed record SaveQuestionsRequest(List<QuestionItemRequest> Questions);

public sealed record QuestionItemRequest(
    int QuestionIndex,
    int X,
    int Y,
    int Width,
    int Height,
    string ModelAnswer,
    double Mark);
