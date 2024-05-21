using Application.Common;

namespace Application.Domain.Portfolios;
public class Portfolio : BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; init; }
}