using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PortfolioLab.Infrastructure;

public class PriceDataDbContext: IdentityDbContext
{
    public DbSet<PriceBarEntity> Price => Set<PriceBarEntity>();
    public DbSet<SavedPortfolioEntity> SavedPortfolios => Set<SavedPortfolioEntity>();
    public PriceDataDbContext(DbContextOptions<PriceDataDbContext> options): base(options){}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}