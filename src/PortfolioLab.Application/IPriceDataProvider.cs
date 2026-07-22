using PortfolioLab.Domain;

namespace PortfolioLab.Application;

public interface IPriceDataProvider
{
    IReadOnlyList<PriceBar> GetPrices(string ticket);
}