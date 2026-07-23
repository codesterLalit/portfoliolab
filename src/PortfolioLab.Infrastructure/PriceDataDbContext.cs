using Microsoft.EntityFrameworkCore;

namespace PortfolioLab.Infrastructure;

public class PriceDataDbContext: DbContext
{
    public DbSet<PriceBarEntity> Price => Set<PriceBarEntity>();
    public PriceDataDbContext(DbContextOptions<PriceDataDbContext> options): base(options){}
    public DbSet<SavedPortfolioEntity> SavedPortfolios => Set<SavedPortfolioEntity>();
}