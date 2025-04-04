﻿using Application.Common;
using Application.Domain.Portfolios;
using Application.Infrastructure;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Features.Portfolios;

[Route("api/portfolio")]
public class CreatePortfolioController(IMediator mediator, ILogger<CreatePortfolioController> logger) : ApiControllerBase(mediator, logger)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePortfolioCommand command)
    {
        return await ApiResult(command);
    }
}

public record CreatePortfolioCommand(string Name) : IRequest<IResult>;

public class CreatePortfolioCommandHandler(PortfolioDbContext portfolioDbContext) : IRequestHandler<CreatePortfolioCommand, IResult>
{
    private readonly PortfolioDbContext _portfolioDbContext = portfolioDbContext;

    public async Task<IResult> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
    {
        var porfolio = new Portfolio
        {
            Name = request.Name
        };

        await _portfolioDbContext.Portfolios.AddAsync(porfolio, cancellationToken);
        await _portfolioDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}