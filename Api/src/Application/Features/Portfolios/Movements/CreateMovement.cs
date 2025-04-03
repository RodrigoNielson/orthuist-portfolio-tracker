using Application.Common;
using Application.Domain.Portfolios;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Portfolios.Movements;

[Route("api/portfolio/asset/movement")]
public class CreateAssetMovementController(IMediator mediator, ILogger<CreateAssetMovementController> logger) : ApiControllerBase(mediator, logger)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetMovementCommand command)
    {
        return await ApiResult(command);
    }
}

public record CreateAssetMovementCommand(
    Guid PortfolioId,
    Guid AssetId,
    decimal Price,
    decimal Quantity,
    DateTime MovementDate,
    MovementType MovementType) : IRequest<IResult>;

public class CreateMovementUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<CreateAssetMovementCommand, IResult>
{
    public async Task<IResult> Handle(CreateAssetMovementCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioDbContext.Portfolios
            .FirstOrDefaultAsync(c => c.Id == request.PortfolioId, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        await portfolioDbContext.Entry(portfolio)
            .Collection(c => c.Assets)
            .Query()
            .Where(c => c.Id == request.AssetId)
            .Include(c => c.Movements)
            .LoadAsync(cancellationToken: cancellationToken);

        var result = portfolio.CreateMovement(request.AssetId, request.Price, request.Quantity, request.MovementDate, request.MovementType);

        if (!result.IsSuccess)
            return result;

        await portfolioDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
