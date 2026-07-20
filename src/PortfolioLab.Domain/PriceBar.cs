namespace PortfolioLab.Domain;

public readonly record struct PriceBar(
    DateOnly Date,
    double Close
);