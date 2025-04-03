using Application.Common;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Portfolios;

[Route("api/portfolio")]
public class DeletePortfolioController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> Create(Guid id)
    {
        var command = new DeletePortfolioCommand(id);   

        return await ApiResult(command);
    }
}

public class DeletePortfolioUseCase(PortfolioDbContext portfolioDbContext) : IRequestHandler<DeletePortfolioCommand, IResult>
{
    public async Task<IResult> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await portfolioDbContext.Portfolios
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (portfolio == null)
            return Result.Error("Portfolio not found");

        portfolioDbContext.Portfolios.Remove(portfolio);

        var result = await portfolioDbContext.SaveChangesAsync(cancellationToken);

        if (result > 0)
            return Result.Success();

        return Result.Error("Error deleting portfolio");
    }
}

public record DeletePortfolioCommand(Guid Id) : IRequest<IResult>;