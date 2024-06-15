using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.Common;

[ApiController]
public class ApiControllerBase(IMediator mediator) : ControllerBase
{
    public readonly IMediator _mediator = mediator;

    public async Task<IActionResult> ApiResult<T>(IRequest<T> command) where T : Result
    {
        try
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                if (result.Value == null)
                    return NoContent();

                return Ok(result.Value);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
