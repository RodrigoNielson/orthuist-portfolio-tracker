using Application.Common;
using Ardalis.Result;

namespace Application.Domain.Portfolios;
public class Portfolio : BaseEntity
{
    public string Name { get; set; }
    
    public IList<PortfolioAsset> Assets { get; set; } = new List<PortfolioAsset>();

    public Result AddAsset(string code, string name, PortfolioAssetType type, decimal price, decimal quantity)
    {
        if (Assets.Any(c => c.Code == code))
            return Result.Conflict("Asset already exists");

        var portfolioAsset = new PortfolioAsset
        {
            PortfolioId = Id,
            Code = code,
            Name = name,
            Type = type
        };

        portfolioAsset.AddMovement(price, quantity, MovementType.Add);

        Assets.Add(portfolioAsset);

        return Result.Success();
    }
}