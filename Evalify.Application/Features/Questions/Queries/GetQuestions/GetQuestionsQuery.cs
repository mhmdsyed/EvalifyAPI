using Evalify.Application.Features.Questions.Dtos;
using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Questions.Queries.GetQuestions;

public sealed record GetQuestionsQuery(int TemplateId) : IRequest<Result<List<QuestionDto>>>;
