namespace PortfolioLab.Domain;

public readonly record struct Holding(string Ticker, IReadOnlyList<PriceBar> PriceHistory, double Weight);