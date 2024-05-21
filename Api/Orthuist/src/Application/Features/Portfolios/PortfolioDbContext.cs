using Application.Domain.Portfolios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Portfolios;
public class PortfolioDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }

    private readonly IConfiguration _configuration = configuration;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Portfolio>(p =>
            {
                p.ToTable(nameof(Portfolio).ToLower());
                p.HasKey(c => c.Id);
            });
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PortfolioContext"))
                         .UseSnakeCaseNamingConvention();
}