using PortfolioLab.Application;
using PortfolioLab.Domain;

namespace PortfolioLab.Infrastructure;

public class EfPriceDataProvider: IPriceDataProvider
{
    private readonly PriceDataDbContext _db;

    public EfPriceDataProvider(PriceDataDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<PriceBar> GetPrices(string ticker)
    {
        return _db.Price
                .Where(p => p.Ticker ==  ticker)
                .OrderBy(p=> p.Date)
                .Select(p => new PriceBar(p.Date, p.Close))
                .ToList();
    }
}