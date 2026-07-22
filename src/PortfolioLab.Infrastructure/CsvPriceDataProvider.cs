using PortfolioLab.Domain;
using PortfolioLab.Application;

namespace PortfolioLab.Infrastructure;

public sealed class CsvPriceDataProvider: IPriceDataProvider
{
  private readonly string _folderPath;

  public CsvPriceDataProvider(string folderPath)
  {
    _folderPath = folderPath;
  } 
   public IReadOnlyList<PriceBar> GetPrices(string ticker)
    {
        string filePath = Path.Combine(_folderPath, $"{ticker}.csv");

        if(!File.Exists(filePath)) throw new FileNotFoundException($"No price data found for ticker '{ticker}'", filePath);

        var lines = File.ReadAllLines(filePath);
        var prices = new List<PriceBar>();

        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split(',');
            var date = DateOnly.Parse(parts[0]);
            var close = double.Parse(parts[1]);

            prices.Add(new PriceBar(date, close));
        }

        return prices;
    } 
}