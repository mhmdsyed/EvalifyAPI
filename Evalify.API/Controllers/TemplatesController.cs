using Asp.Versioning;
using Evalify.API.Contracts.Requests;
using Evalify.Application.Features.Questions.Commands.SaveQuestions;
using Evalify.Application.Features.Questions.Dtos;
using Evalify.Application.Features.Questions.Queries.GetQuestions;
using Evalify.Application.Features.Papers.Commands.UploadPaper;
using Evalify.Application.Features.Papers.Dtos;
using Evalify.Application.Features.Papers.Queries.GetPapers;
using Evalify.Application.Features.Templates.Commands.CreateTemplate;
using Evalify.Application.Features.Templates.Dtos;
using Evalify.Application.Features.Templates.Queries.ExportResults;
using Evalify.Application.Features.Templates.Queries.GetTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/templates")]
[ApiVersion("1.0")]
public sealed class TemplatesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<TemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Get all templates for the current teacher.")]
    [EndpointName("GetTemplates")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetTemplatesQuery(), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Upload a new template image.")]
    [EndpointName("CreateTemplate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromForm] string name,
        IFormFile image,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateTemplateCommand(name, image.OpenReadStream(), image.FileName), ct);

        return result.Match(Ok, Problem);
    }

    [HttpGet("{templateId:int}/questions")]
    [ProducesResponseType(typeof(List<QuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get bounding box questions for a template.")]
    [EndpointName("GetQuestions")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetQuestions(int templateId, CancellationToken ct)
    {
        var result = await sender.Send(new GetQuestionsQuery(templateId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPost("{templateId:int}/questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Save bounding boxes for a template (full replace).")]
    [EndpointName("SaveQuestions")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> SaveQuestions(
        int templateId,
        SaveQuestionsRequest request,
        CancellationToken ct)
    {
        var questions = request.Questions
            .Select(q => new QuestionItem(
                q.QuestionIndex, q.X, q.Y, q.Width, q.Height, q.ModelAnswer, q.Mark))
            .ToList();

        var result = await sender.Send(new SaveQuestionsCommand(templateId, questions), ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpGet("{templateId:int}/papers")]
    [ProducesResponseType(typeof(List<PaperDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get all student papers for a template.")]
    [EndpointName("GetPapers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetPapers(int templateId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPapersQuery(templateId), ct);
        return result.Match(Ok, Problem);
    }

   [HttpPost("{templateId:int}/papers")]
[Consumes("multipart/form-data")]
[RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
[ProducesResponseType(StatusCodes.Status202Accepted)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[EndpointSummary("Upload one or more student answer sheets.")]
[EndpointName("UploadPapers")]
[MapToApiVersion("1.0")]
public async Task<IActionResult> UploadPapers(
    int templateId,
    [FromForm] IFormFileCollection images,
    CancellationToken ct)
    {
        var files = Request.Form.Files.Count > 0
    ? Request.Form.Files.Cast<IFormFile>().ToList()
    : images?.ToList() ?? [];
    
        if (files.Count == 0)
            return BadRequest("At least one image is required.");

        var responses = new List<object>();
        var errors = new List<object>();

        foreach (var image in files)
        {
            var result = await sender.Send(
                new UploadPaperCommand(templateId, image.OpenReadStream(), image.FileName), ct);

            if (result.IsError)
            {
                errors.Add(new { fileName = image.FileName, error = result.TopError.Description });
                continue;
            }

            responses.Add(new
            {
                studentPaperId = result.Value.StudentPaperId,
                studentCode = result.Value.StudentCode,
                status = result.Value.Status
            });
        }

        return Accepted(new { uploaded = responses, failed = errors });
    }

    [HttpGet("{templateId:int}/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Export all students results as CSV.")]
    [EndpointName("ExportResults")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Export(int templateId, CancellationToken ct)
    {
        var result = await sender.Send(new ExportResultsQuery(templateId), ct);

        return result.Match(
            response => File(response.FileContent, response.ContentType, response.FileName),
            Problem);
    }
}
