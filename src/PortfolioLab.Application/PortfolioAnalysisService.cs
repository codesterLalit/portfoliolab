using PortfolioLab.Domain;

namespace PortfolioLab.Application;

public class PortfolioAnalysisService
{
    private readonly IPriceDataProvider _priceDataProvider;

    public PortfolioAnalysisService(IPriceDataProvider priceDataProvider)
    {
        _priceDataProvider = priceDataProvider;
    }
    
    public PortfolioReport GenerateReport(string ticker, double riskFreeRate)
    {
        var prices = _priceDataProvider.GetPrices(ticker);

        return new PortfolioReport(
            AnnualisedVolatility: Analytics.Volatility(prices),
            SharpeRatio: Analytics.Sharpe(prices, riskFreeRate),
            MaxDrawdown: Analytics.MaxDrawdown(prices),
            HistoricalVaR95: Analytics.HistoricalVaR95(prices)
        );
    }
}