using Application.Domain.Portfolios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Portfolios;
public class PortfolioDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<PortfolioAsset> Assets { get; set; }
    public DbSet<Movement> Movements { get; set; }

    private readonly IConfiguration _configuration = configuration;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Portfolio>(p =>
        {
            p.ToTable(nameof(Portfolio).ToLower());
            p.HasKey(c => c.Id);
            p.HasMany(c => c.Assets);
        });

        modelBuilder.Entity<PortfolioAsset>(p =>
        {
            p.ToTable(nameof(PortfolioAsset).ToLower());
            p.HasKey(c => c.Id);
            p.HasMany(c => c.Movements);
        });

        modelBuilder.Entity<Movement>(p =>
        {
            p.ToTable(nameof(Movement).ToLower());
            p.HasKey(c => c.Id);
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PortfolioContext"))
                         .UseSnakeCaseNamingConvention();
}