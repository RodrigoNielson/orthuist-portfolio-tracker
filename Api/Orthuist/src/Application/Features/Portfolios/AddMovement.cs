using Application.Common;
using Application.Domain.Portfolios;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Portfolios;

[Route("api/portfolio/asset/movement")]
public class CreateAssetMovementController(IMediator mediator) : ApiControllerBase(mediator)
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
    MovementType MovementType) : IRequest<IResult>;

public class AddMovementUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<CreateAssetMovementCommand, IResult>
{
    private readonly PortfolioDbContext _portfolioDbContext = portfolioDbContext;

    public async Task<IResult> Handle(CreateAssetMovementCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioDbContext.Portfolios
            .SingleAsync(c => c.Id == request.PortfolioId, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        await _portfolioDbContext.Entry(portfolio)
            .Collection(c => c.Assets)
            .Query()
            .Where(c => c.Id == request.AssetId)
            .Include(c => c.Movements)
            .LoadAsync(cancellationToken: cancellationToken);

        var result = portfolio.CreateMovement(request.AssetId, request.Price, request.Quantity, request.MovementType);

        if (!result.IsSuccess)
            return result;

        await _portfolioDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
