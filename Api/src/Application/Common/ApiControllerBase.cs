using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Common;

[ApiController]
public class ApiControllerBase(IMediator mediator, ILogger logger) : ControllerBase
{
    public readonly IMediator _mediator = mediator;

    public async Task<IActionResult> ApiResult<T>(IRequest<T> command) where T : IResult
    {
        try
        {
            var result = await _mediator.Send(command);

            if (result.Status == ResultStatus.Ok)
            {
                if (result.ValueType == null)
                    return NoContent();

                return Ok(result.GetValue());
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing the command {comand} request: {Message}", nameof(command), ex.Message);

            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
