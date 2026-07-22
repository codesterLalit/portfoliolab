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
        public static double MaxDrawdown(double[] returns)
    {
        var cumulativeValues = new double[returns.Length];
        double cumulativeValue = 1.0;

        for(int i=0; i< returns.Length; i++)
        {
            cumulativeValue *= 1 + returns[i];
            cumulativeValues[i] = cumulativeValue;
        }

        double worstDrawdown = 0;
        double runningMax = cumulativeValues[0];

        foreach(var returnComulativeValue in cumulativeValues)
        {
            runningMax = Math.Max(runningMax, returnComulativeValue);
            double drawdown = (returnComulativeValue - runningMax) / runningMax;
            worstDrawdown = Math.Min(worstDrawdown, drawdown);
        }
        return worstDrawdown;  
    }

    public static double Volatility(double[] returns)
    {
        double mean = returns.Average();

        double sumSquaredDiffs = returns.Sum(r => Math.Pow(r-mean, 2));
        double dailyStdDev = Math.Sqrt(sumSquaredDiffs / (returns.Length - 1));

        return dailyStdDev * Math.Sqrt(252); // Annualize assuming 252 trading days      
    }

        public static double Volatility(IReadOnlyList<PriceBar> price)
    {
        var returns =  SimpleReturns(price);
        double mean = returns.Average();

        double sumSquaredDiffs = returns.Sum(r => Math.Pow(r-mean, 2));
        double dailyStdDev = Math.Sqrt(sumSquaredDiffs / (returns.Length - 1));

        return dailyStdDev * Math.Sqrt(252); // Annualize assuming 252 trading days      
    }

    public static double Sharpe(double[] returns, double riskFreeRate)
    {
        double meanDailyReturn = returns.Average();
        double annualReturn = meanDailyReturn * 252;

        double annualVol = Volatility(returns);

        return (annualReturn - riskFreeRate) / annualVol;
    }

        public static double Sharpe(IReadOnlyList<PriceBar> price, double riskFreeRate)
    {
        var returns = SimpleReturns(price);
        double meanDailyReturn = returns.Average();
        double annualReturn = meanDailyReturn * 252;

        double annualVol = Volatility(returns);

        return (annualReturn - riskFreeRate) / annualVol;
    }

    public static double HistoricalVaR95(double[] returns)
    {
        var sorted = returns.OrderBy(r => r).ToArray();

        // 5th percentile, linear interpolation
        double position = 0.05 * (sorted.Length - 1);
        int lower = (int) Math.Floor(position);
        int upper = (int) Math.Ceiling(position);


        if(lower == upper) return sorted[lower];

        double fraction = position - lower;
        return sorted[lower] + fraction * (sorted[upper] - sorted[lower]);
    }

        public static double HistoricalVaR95(IReadOnlyList<PriceBar> price)
    {
        var returns = SimpleReturns(price);
        var sorted = returns.OrderBy(r => r).ToArray();

        // 5th percentile, linear interpolation
        double position = 0.05 * (sorted.Length - 1);
        int lower = (int) Math.Floor(position);
        int upper = (int) Math.Ceiling(position);


        if(lower == upper) return sorted[lower];

        double fraction = position - lower;
        return sorted[lower] + fraction * (sorted[upper] - sorted[lower]);
    }

    public static double Correlation(IReadOnlyList<PriceBar> priceA, IReadOnlyList<PriceBar> priceB)
    {
        var returnsA = SimpleReturns(priceA);
        var returnsB = SimpleReturns(priceB);

        double meanA = returnsA.Average();
        double meanB = returnsB.Average();

        double covariance = returnsA.Zip(returnsB, (a, b) => (a - meanA) * (b - meanB)).Sum() / (returnsA.Length -1);

        double stdA = Math.Sqrt(returnsA.Sum(a => Math.Pow(a - meanA, 2)) / (returnsA.Length - 1));
        double stdB = Math.Sqrt(returnsB.Sum(b => Math.Pow(b - meanB, 2)) / (returnsB.Length - 1));

        return covariance / (stdA * stdB);
    }

    public static double[] PortfolioReturns(Portfolio portfolio)
    {
        var perAssetsReturns = portfolio.Holdings
                .Select(h => SimpleReturns(h.PriceHistory))
                .ToArray();
        int days = perAssetsReturns[0].Length;
        var blended = new double[days];

        for(int day = 0; day < days; day++)
        {
            for(int i=0; i < portfolio.Holdings.Count; i++)
            {
                blended[day] += portfolio.Holdings[i].Weight *  perAssetsReturns[i][day];
            }
        }
        return blended;
    }
}