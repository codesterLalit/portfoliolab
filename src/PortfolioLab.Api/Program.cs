using PortfolioLab.Application;
using PortfolioLab.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPriceDataProvider>(_ => 
    new CsvPriceDataProvider(@"c:\PortfolioData")
);

builder.Services.AddScoped<PortfolioAnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/portfolio/report", (string tickers, double riskFreeRate, PortfolioAnalysisService service) =>
{
    var tickerWeights = tickers.Split(",")
        .Select(pair => pair.Split(":"))
        .ToDictionary(parts => parts[0], parts => double.Parse(parts[1]));

    var report = service.GenerateReport(tickerWeights, riskFreeRate);
    return Results.Ok(report);
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
