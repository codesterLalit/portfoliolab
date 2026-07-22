using PortfolioLab.Domain;

namespace PortfolioLab.Application;

public class PortfolioAnalysisService
{
    private readonly IPriceDataProvider _priceDataProvider;

    public PortfolioAnalysisService(IPriceDataProvider priceDataProvider)
    {
        _priceDataProvider = priceDataProvider;
    }
    
    public PortfolioReport GenerateReport(Dictionary<string, double> tickerWeights, double riskFreeRate)
    {

        var holdings = tickerWeights.Select(tw =>
        {
            var Ticker = tw.Key;
            var Weight = tw.Value;
            var pricePortfolio = _priceDataProvider.GetPrices(Ticker);
            return new Holding(Ticker, pricePortfolio, Weight);
        }).ToList();

        var portofolio = new Portfolio(holdings);

        var blendedReturns = Analytics.PortfolioReturns(portofolio);

        return new PortfolioReport(
            AnnualisedVolatility: Analytics.Volatility(blendedReturns),
            SharpeRatio: Analytics.Sharpe(blendedReturns, riskFreeRate),
            MaxDrawdown: Analytics.MaxDrawdown(blendedReturns),
            HistoricalVaR95: Analytics.HistoricalVaR95(blendedReturns)
        );
    }
}