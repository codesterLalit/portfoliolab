using PortfolioLab.Domain;

namespace PortfolioLab.Application.Tests;

public class PortfolioAnalysisServiceTests
{
    // [Fact]
    // public void GenerateReport_Combines_All_Metrics_Correctly()
    // {
    //     var portfolio = new Portfolio(
    //         [
    //         new(new DateOnly(2024, 1, 1), 100),
    //         new(new DateOnly(2024, 1, 2), 102),
    //         new(new DateOnly(2024, 1, 3), 101),
    //         new(new DateOnly(2024, 1, 4), 105),
    //         new(new DateOnly(2024, 1, 5), 103)
    //         ]
    //     );

    //     var report = PortfolioAnalysisService.GenerateReport(iskFreeRate: 0.02);

    //     Assert.Equal(0.4290, report.AnnualisedVolatility, precision: 3);
    //     Assert.Equal(4.4695, report.SharpeRatio, precision: 4);
    //     Assert.Equal(-0.019048, report.MaxDrawdown, precision: 4);
    //     Assert.Equal(-0.017661, report.HistoricalVaR95, precision: 4);
    // }
}