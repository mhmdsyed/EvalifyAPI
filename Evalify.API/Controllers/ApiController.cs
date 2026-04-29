using Evalify.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Evalify.API.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
            return Problem();

        if (errors.All(e => e.Type == ErrorKind.Validation))
            return ValidationProblem(errors);

        return Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorKind.Conflict     => StatusCodes.Status409Conflict,
            ErrorKind.Validation   => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound     => StatusCodes.Status404NotFound,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden    => StatusCodes.Status403Forbidden,
            _                      => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        var modelState = new ModelStateDictionary();
        errors.ForEach(e => modelState.AddModelError(e.Code, e.Description));
        return ValidationProblem(modelState);
    }
}
