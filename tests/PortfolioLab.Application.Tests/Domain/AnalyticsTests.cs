using PortfolioLab.Domain;

namespace PortfolioLab.Application.Tests.Domain;

public class AnalyticsTests
{
    private static readonly PriceBar[] SamplePrices =
    [
        new(new DateOnly(2024, 1, 1), 100),
        new(new DateOnly(2024, 1, 2), 102),
        new(new DateOnly(2024, 1, 3), 101),
        new(new DateOnly(2024, 1, 4), 105),
        new(new DateOnly(2024, 1, 5), 103)
        // replace with the exact prices from your scratch project's worked example
    ];

    [Fact]
    public void Sharpe_Matches_HandCalculated_Value()
    {
        double riskFreeRate = 0.02; // use whatever you used by hand
        double result = Analytics.Sharpe(SamplePrices, riskFreeRate);

        Assert.Equal(4.4695, result, precision: 4);
    }

    [Fact]
    public void Volatility_Matches_HandCalculated_Value()
    {
        double result = Analytics.Volatility(SamplePrices);
        Assert.Equal(0.4290, result, precision: 3);
    }

    [Fact]
    public void MaxDrawdown_Matches_HandCalculated_Value()
    {
        double result = Analytics.MaxDrawdown(SamplePrices);
        Assert.Equal(-0.019048, result, precision: 4);
    }

    [Fact]
    public void HistoricalVaR95_Matches_HandCalculated_Value()
    {
        double result = Analytics.HistoricalVaR95(SamplePrices);
        Assert.Equal(-0.017661, result, precision: 4);
    }

    [Fact]
    public void Correlation_Matches_HandCalculated_Value()
    {
        PriceBar[] msft =
        [
            new(new DateOnly(2024,1,1), 200),
        new(new DateOnly(2024,1,2), 198),
        new(new DateOnly(2024,1,3), 202),
        new(new DateOnly(2024,1,4), 199),
        new(new DateOnly(2024,1,5), 205)
        ];

        double result = Analytics.Correlation(SamplePrices, msft);
        Assert.Equal(-0.977, result, precision: 2);
    }
}