using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Application.Features.Papers.Dtos;
using Evalify.Application.Features.Papers.Mappers;
using Evalify.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Papers.Queries.GetPapers;

public sealed class GetPapersQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<GetPapersQuery, Result<List<PaperDto>>>
{
    public async Task<Result<List<PaperDto>>> Handle(
        GetPapersQuery request,
        CancellationToken ct)
    {
        var template = await db.Templates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, ct);

        if (template is null)
            return ApplicationErrors.TemplateNotFound;

        if (template.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        var papers = await db.StudentPapers
            .Where(p => p.TemplateId == request.TemplateId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        return papers.ToDtos();
    }
}
