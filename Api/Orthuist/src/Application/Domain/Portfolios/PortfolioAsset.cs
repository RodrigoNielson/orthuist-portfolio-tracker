using Application.Common;
using Ardalis.Result;
using EntityFrameworkCore.Projectables;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Domain.Portfolios;
public class PortfolioAsset : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public PortfolioAssetType Type { get; set; }
    public IList<Movement> Movements { get; set; } = new List<Movement>();

    [NotMapped, Projectable]
    public decimal AllocatedQuantity => Movements.Sum(c => c.Type == MovementType.Add ? c.Quantity : c.Quantity * -1);

    public Result CreateMovement(decimal price, decimal quantity, MovementType movementType)
    {
        return movementType switch
        {
            MovementType.Add => AddMovement(price, quantity),
            MovementType.Subtract => SubtractMovement(price, quantity),
            _ => Result.Error("Invalid movement type"),
        };
    }

    private Result AddMovement(decimal price, decimal quantity)
    {
        Movements.Add(new Movement
        {
            Price = price,
            Quantity = quantity,
            Type = MovementType.Add
        });

        return Result.Success();
    }

    private Result SubtractMovement(decimal price, decimal quantity)
    {
        if (AllocatedQuantity - quantity < 0)
            return Result.Error("Allocated quantity cannot be lower than 0");

        Movements.Add(new Movement
        {
            Price = price,
            Quantity = quantity,
            Type = MovementType.Subtract
        });

        return Result.Success();
    }
}

public enum PortfolioAssetType : short
{
    Stock,
    FixedIncome
}