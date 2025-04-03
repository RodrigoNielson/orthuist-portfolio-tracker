using Application.Common;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Portfolios.Movements;

[Route("api/portfolio")]
public class DeleteAssetMovementController(IMediator mediator, ILogger<DeleteAssetMovementController> logger) : ApiControllerBase(mediator, logger)
{
    [HttpDelete("{portfolioId}/asset/{assetId}/movement/{movementId}")]
    public async Task<IActionResult> Delete(Guid portfolioId, Guid assetId, Guid movementId)
    {
        var command = new DeleteAssetMovementCommand(portfolioId, assetId, movementId);
        return await ApiResult(command);
    }
}

public record DeleteAssetMovementCommand(
    Guid PortfolioId,
    Guid AssetId,
    Guid MovementId) : IRequest<IResult>;

public class DeleteMovementUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<DeleteAssetMovementCommand, IResult>
{
    public async Task<IResult> Handle(DeleteAssetMovementCommand request, CancellationToken cancellationToken)
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


        var deleteMovementResult = portfolio.DeleteMovement(request.AssetId, request.MovementId);

        if (!deleteMovementResult.IsSuccess)
            return deleteMovementResult;

        var result = await portfolioDbContext.SaveChangesAsync(cancellationToken);

        if (result > 0)
            return Result.Success();

        return Result.Error("Error deleting movement");
    }
}