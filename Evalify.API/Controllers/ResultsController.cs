using Asp.Versioning;
using Evalify.API.Contracts.Requests;
using Evalify.Application.Features.Results.Commands.AdjustGrade;
using Evalify.Application.Features.Results.Dtos;
using Evalify.Application.Features.Results.Queries.GetResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}")]
[ApiVersion("1.0")]
public sealed class ResultsController(ISender sender) : ApiController
{
    [HttpGet("papers/{studentPaperId:int}/results")]
    [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get detailed evaluation results for a student paper.")]
    [EndpointName("GetResults")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetResults(int studentPaperId, CancellationToken ct)
    {
        var result = await sender.Send(new GetResultsQuery(studentPaperId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("answers/{answerId:int}/grade")]
    [ProducesResponseType(typeof(AdjustGradeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Manually adjust a student answer grade.")]
    [EndpointName("AdjustGrade")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AdjustGrade(
        int answerId,
        AdjustGradeRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new AdjustGradeCommand(answerId, request.Grade), ct);
        return result.Match(Ok, Problem);
    }
}
