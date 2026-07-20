namespace PortfolioLab.Domain;

public sealed class Portfolio
{
    public IReadOnlyList<PriceBar> PriceHistory {get;}

    public Portfolio(IReadOnlyList<PriceBar> priceHistory)
    {
       if(priceHistory is null || priceHistory.Count < 2)
        {
            throw new ArgumentException("Need at least two price points.", nameof(priceHistory));
        }
        PriceHistory = priceHistory;   
    }
}