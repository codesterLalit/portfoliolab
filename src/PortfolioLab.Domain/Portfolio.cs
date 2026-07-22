namespace PortfolioLab.Domain;

public sealed class Portfolio
{
    public IReadOnlyList<Holding> Holdings {get;}

    public Portfolio(IReadOnlyList<Holding> holdings)
    {
        if(holdings is null || holdings.Count == 0){
            throw new ArgumentException("Portfolio needs at least one holding.");
        }

        double totalWeight = holdings.Sum(h=> h.Weight);

        if(Math.Abs(totalWeight - 1.0) > 0.0001)
        {
            throw new ArgumentException($"Weights must sum to 1.0 (got {totalWeight})", nameof(holdings));
        }

        Holdings = holdings;
    }
}