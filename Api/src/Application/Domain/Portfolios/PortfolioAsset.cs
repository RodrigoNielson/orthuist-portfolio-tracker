﻿using Application.Common;
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
    public IList<Movement> Movements { get; set; } = [];

    [NotMapped, Projectable]
    public decimal AllocatedQuantity => Movements.Sum(c => c.Type == MovementType.Add ? c.Quantity : c.Quantity * -1);

    public Result CreateMovement(decimal price, decimal quantity, DateTime date, MovementType movementType)
    {
        return movementType switch
        {
            MovementType.Add => CreateAddMovement(price, quantity, date),
            MovementType.Subtract => CreateSubtractMovement(price, quantity, date),
            _ => Result.Error("Invalid movement type"),
        };
    }

    public Result DeleteMovement(Guid movementId)
    {
        var movement = Movements.FirstOrDefault(c => c.Id == movementId);

        if (movement == null)
            return Result.NotFound("Movement not found");

        if (AllocatedQuantity - movement.Quantity < 0)
            return Result.Error("Allocated quantity cannot be lower than 0");

        Movements.Remove(movement);

        return Result.Success();
    }

    private Result CreateAddMovement(decimal price, decimal quantity, DateTime date)
    {
        Movements.Add(new Movement
        {
            Price = price,
            Quantity = quantity,
            Type = MovementType.Add,
            MovementDate = date
        });

        return Result.Success();
    }

    private Result CreateSubtractMovement(decimal price, decimal quantity, DateTime date)
    {
        if (AllocatedQuantity - quantity < 0)
            return Result.Error("Allocated quantity cannot be lower than 0");

        Movements.Add(new Movement
        {
            Price = price,
            Quantity = quantity,
            Type = MovementType.Subtract,
            MovementDate = date
        });

        return Result.Success();
    }
}

public enum PortfolioAssetType : short
{
    Stock,
    FixedIncome
}