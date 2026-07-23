using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioLab.Application;
using PortfolioLab.Domain;

namespace PortfolioLab.Infrastructure;

public class StooqLivePriceDataProvider: IPriceDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly PriceDataDbContext _db;

    public StooqLivePriceDataProvider(HttpClient httpClient, PriceDataDbContext db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    public IReadOnlyList<PriceBar> GetPrices(string ticker)
    {
        var cached = _db.Price
                .Where(p => p.Ticker == ticker)
                .OrderBy(p => p.Date)
                .ToList();
        
        if(cached.Count > 0)
            return cached.Select(p => new PriceBar(p.Date, p.Close)).ToList();
        
        var fetched = FetchFromStooq(ticker);

        _db.Price.AddRange(fetched.Select(p => new PriceBarEntity
        {
            Ticker = ticker,
            Date = p.Date,
            Close = p.Close
        }));

        _db.SaveChanges();
        return fetched;
    }

    private List<PriceBar> FetchFromStooq(string ticker)
    {
    string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}?range=1y&interval=1d";

    string json = _httpClient.GetStringAsync(url).GetAwaiter().GetResult();
    using var doc = JsonDocument.Parse(json);

    var result = doc.RootElement.GetProperty("chart").GetProperty("result")[0];
    var timestamps = result.GetProperty("timestamp").EnumerateArray().Select(t => t.GetInt64()).ToArray();
    var closes = result.GetProperty("indicators").GetProperty("quote")[0].GetProperty("close").EnumerateArray().ToArray();

    var prices = new List<PriceBar>();
    for(int i = 0; i < timestamps.Length; i++)
        {
            if(closes[i].ValueKind == JsonValueKind.Null) continue;

            var date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).UtcDateTime);
            var close = closes[i].GetDouble();
            prices.Add(new PriceBar(date, close));
        }

        if(prices.Count == 0)
        {
            throw new InvalidOperationException($"No live price data found for '{ticker}'.");
        }

        return prices;
    }


}