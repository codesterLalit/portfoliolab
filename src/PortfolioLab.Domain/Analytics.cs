namespace PortfolioLab.Domain;

public static class Analytics
{
    public static double[] SimpleReturns(IReadOnlyList<PriceBar> prices)
    {
        var returns = new double[prices.Count - 1];

        for(int i = 1; i < prices.Count; i++)
        {
            returns[i -1] = (prices[i].Close - prices[i - 1].Close) / prices[i - 1].Close;
        }        
        return returns;
    }
}