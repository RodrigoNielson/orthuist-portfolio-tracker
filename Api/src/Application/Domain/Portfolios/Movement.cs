using Application.Common;

namespace Application.Domain.Portfolios;
public class Movement : BaseEntity
{
    public Guid PortfolioAssetId { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime MovementDate { get; set; }
    public MovementType Type { get; set; }
}

public enum MovementType : short
{
    Add,
    Subtract
}