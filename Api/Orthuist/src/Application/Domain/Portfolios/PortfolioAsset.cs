using Application.Common;

namespace Application.Domain.Portfolios;
public class PortfolioAsset : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public PortfolioAssetType Type { get; set; }

    public IList<Movement> Movements { get; set; } = new List<Movement>();

    public void AddMovement(decimal price, decimal quantity, MovementType type)
    {
        Movements.Add(new Movement
        {
            Price = price,
            Quantity = quantity,
            Type = type
        });
    }
}

public enum PortfolioAssetType : short
{
    Stocks,
    FixedIncome
}