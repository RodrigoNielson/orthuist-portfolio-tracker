using Application.Common;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Portfolios;

[Route("api/portfolio")]
public class GetPortfolioController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get(GetPortfolioCommand command)
    {
        return await ApiResult(command);
    }
}

public record GetPortfolioCommand(Guid Id) : IRequest<IResult>;

// use Case


public record PortfolioDto
{
    public int MyProperty { get; set; }

};