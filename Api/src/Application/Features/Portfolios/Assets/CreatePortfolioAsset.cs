using Application.Common;
using Application.Domain.Portfolios;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Portfolios.Assets;

[Route("api/portfolio/asset")]
public class CreatePortfolioAssetController(IMediator mediator, ILogger<CreatePortfolioAssetController> logger) : ApiControllerBase(mediator, logger)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePortfolioAssetCommand command)
    {
        return await ApiResult(command);
    }
}

public record CreatePortfolioAssetCommand(
    Guid PortfolioId,
    string Code,
    string Name,
    PortfolioAssetType Type,
    decimal Price,
    decimal Quantity,
    DateTime MovementDate) : IRequest<IResult>;

public class CreatePortfolioUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<CreatePortfolioAssetCommand, IResult>
{
    private readonly PortfolioDbContext _portfolioDbContext = portfolioDbContext;

    public async Task<IResult> Handle(CreatePortfolioAssetCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioDbContext.Portfolios
            .Include(c => c.Assets
                .Where(a => a.Code == request.Code))
            .FirstOrDefaultAsync(c => c.Id == request.PortfolioId, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        var result = portfolio.CreateAsset(request.Code, request.Name, request.Type, request.Price, request.Quantity, request.MovementDate);

        if (!result.IsSuccess)
            return result;

        await _portfolioDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
