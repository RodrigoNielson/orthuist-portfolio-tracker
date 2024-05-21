using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Common;

[ApiController]
public class ApiControllerBase(IMediator mediator) : ControllerBase
{
    public readonly IMediator _mediator  = mediator;

    public IActionResult ApiResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value == null)
                return NoContent();

            return Ok(result.Value);
        }

        return BadRequest(result);
    }
}
