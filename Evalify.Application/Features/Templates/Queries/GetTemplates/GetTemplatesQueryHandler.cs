using Evalify.Application.Common.Interfaces;
using Evalify.Application.Features.Templates.Dtos;
using Evalify.Application.Features.Templates.Mappers;
using Evalify.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Templates.Queries.GetTemplates;

public sealed class GetTemplatesQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<GetTemplatesQuery, Result<List<TemplateDto>>>
{
    public async Task<Result<List<TemplateDto>>> Handle(
        GetTemplatesQuery request,
        CancellationToken ct)
    {
        var templates = await db.Templates
            .Where(t => t.UserId == currentUser.Id)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return templates.ToDtos();
    }
}
