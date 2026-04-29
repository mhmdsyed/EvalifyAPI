using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Results.Commands.AdjustGrade;

public sealed record AdjustGradeCommand(
    int AnswerId,
    double NewGrade) : IRequest<Result<AdjustGradeResponse>>;

public sealed record AdjustGradeResponse(
    double NewTotalGrade);
