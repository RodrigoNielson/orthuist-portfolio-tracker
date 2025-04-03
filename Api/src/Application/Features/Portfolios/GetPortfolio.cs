using Application.Common;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Portfolios;

[Route("api/portfolio")]
public class GetPortfolioController(IMediator mediator, ILogger<GetPortfolioController> logger) : ApiControllerBase(mediator, logger)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var command = new GetPortfolioCommand(id);

        return await ApiResult(command);
    }
}

public record GetPortfolioCommand(Guid Id) : IRequest<IResult>;

public class GetPorfolioUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<GetPortfolioCommand, IResult>
{
    public async Task<IResult> Handle(GetPortfolioCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioDbContext.Portfolios
                    .AsNoTracking()
                    .Select(c => new PortfolioDto(c.Id, c.Name))
                    .FirstOrDefaultAsync(cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        return Result.Success(portfolio);
    }
}

public record PortfolioDto(Guid Id, string Name);