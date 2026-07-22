namespace PortfolioLab.Application;

public sealed record PortfolioReport(
    double AnnualisedVolatility,
    double SharpeRatio,
    double MaxDrawdown,
    double HistoricalVaR95
);