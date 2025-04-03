using Application.Common;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Portfolios.Assets;

[Route("api/portfolio")]
public class DeletePortfolioAssetController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpDelete("{portfolioId}/asset/{assetId}")]
    public async Task<IActionResult> Delete(Guid portfolioId, Guid assetId)
    {
        var command = new DeletePortfolioAssetCommand(portfolioId, assetId);

        return await ApiResult(command);
    }
}

public class DeletePortfolioAssetUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<DeletePortfolioAssetCommand, IResult>
{
    private readonly PortfolioDbContext _portfolioDbContext = portfolioDbContext;

    public async Task<IResult> Handle(DeletePortfolioAssetCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioDbContext.Portfolios
            .Include(c => c.Assets
                .Where(a => a.Id == request.AssetId))
            .FirstOrDefaultAsync(c => c.Id == request.PortfolioId, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        var removeAssetResult = portfolio.DeleteAsset(request.AssetId);

        if (!removeAssetResult.IsSuccess)
            return removeAssetResult;

        var result = await _portfolioDbContext.SaveChangesAsync(cancellationToken);

        if (result > 0)
            return Result.Success();

        return Result.Error("Error deleting portfolio asset");
    }
}

public record DeletePortfolioAssetCommand(
    Guid PortfolioId,
    Guid AssetId) : IRequest<IResult>;