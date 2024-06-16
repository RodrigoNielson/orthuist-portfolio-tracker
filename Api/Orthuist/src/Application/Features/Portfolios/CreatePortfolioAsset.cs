using Application.Common;
using Application.Domain.Portfolios;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Portfolios;

[Route("api/portfolio/create-asset")]
public class CreatePortfolioAssetController(IMediator mediator) : ApiControllerBase(mediator)
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
    decimal Quantity) : IRequest<IResult>;

public class CreatePortfolioUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<CreatePortfolioAssetCommand, IResult>
{
    private readonly PortfolioDbContext _portfolioDbContext = portfolioDbContext;

    public async Task<IResult> Handle(CreatePortfolioAssetCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioDbContext.Portfolios
            .Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.Id == request.PortfolioId, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        var result = portfolio.AddAsset(request.Code, request.Name, request.Type, request.Price, request.Quantity);

        if (!result.IsSuccess)
            return result;

        await _portfolioDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
