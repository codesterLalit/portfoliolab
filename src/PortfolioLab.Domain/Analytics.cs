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

    public static double MaxDrawdown(IReadOnlyList<PriceBar> prices)
    {
        double worstDrawdown = 0;
        double runningMax = prices[0].Close;

        foreach(var price in prices)
        {
            runningMax = Math.Max(runningMax, price.Close);
            double drawdown = (price.Close - runningMax) / runningMax;
            worstDrawdown = Math.Min(worstDrawdown, drawdown);
        }
        return worstDrawdown;    
    }

    public static double Volatility(IReadOnlyList<PriceBar> prices)
    {
        var returns = SimpleReturns(prices);
        double mean = returns.Average();

        double sumSquaredDiffs = returns.Sum(r => Math.Pow(r-mean, 2));
        double dailyStdDev = Math.Sqrt(sumSquaredDiffs / (returns.Length - 1));

        return dailyStdDev * Math.Sqrt(252); // Annualize assuming 252 trading days      
    }

    public static double Sharpe(IReadOnlyList<PriceBar> prices, double riskFreeRate)
    {
        var returns = SimpleReturns(prices);
        double meanDailyReturn = returns.Average();
        double annualReturn = meanDailyReturn * 252;

        double annualVol = Volatility(prices);

        return (annualReturn - riskFreeRate) / annualVol;
    }
}