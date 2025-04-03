using Application.Common;
using Ardalis.Result;

namespace Application.Domain.Portfolios;
public class Portfolio : BaseEntity
{
    public string Name { get; set; }

    public IList<PortfolioAsset> Assets { get; set; } = [];

    public Result CreateMovement(Guid assetId, decimal price, decimal quantity, MovementType movementType)
    {
        var asset = Assets.FirstOrDefault(c => c.Id == assetId);

        if (asset == null)
            return Result.NotFound("Asset not found");

        asset.CreateMovement(price, quantity, movementType);

        return Result.Success();
    }

    public Result DeleteMovement(Guid assetId, Guid movementId)
    {
        var asset = Assets.FirstOrDefault(c => c.Id == assetId);

        if (asset == null)
            return Result.NotFound("Asset not found");

        return asset.DeleteMovement(movementId);
    }   

    public Result CreateAsset(string code, string name, PortfolioAssetType type, decimal price, decimal quantity)
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

        portfolioAsset.CreateMovement(price, quantity, MovementType.Add);

        Assets.Add(portfolioAsset);

        return Result.Success();
    }

    public Result DeleteAsset(Guid id)
    {
        var asset = Assets.FirstOrDefault(c => c.Id == id);

        if (asset == null)
            return Result.NotFound("Asset not found");

        Assets.Remove(asset);

        return Result.Success();
    }
}