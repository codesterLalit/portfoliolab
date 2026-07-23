using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioLab.Application;
using PortfolioLab.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PriceDataDbContext>(
    options => options.UseNpgsql("Host=localhost;Port=5433;Database=portfoliolab;Username=postgres;Password=devpassword")
);

builder.Services.AddHttpClient<StooqLivePriceDataProvider>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
});

builder.Services.AddScoped<IPriceDataProvider>(sp => sp.GetRequiredService<StooqLivePriceDataProvider>());

// builder.Services.AddScoped<IPriceDataProvider, EfPriceDataProvider>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateReportQuery).Assembly));

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
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

app.MapGet("/portfolio/report", async (string tickers, double riskFreeRate, IMediator mediator) =>
{
    try
    {
        var tickerWeights = tickers.Split(",")
    .Select(pair => pair.Split(":"))
    .ToDictionary(parts => parts[0], parts => double.Parse(parts[1]));

        // var report = service.GenerateReport(tickerWeights, riskFreeRate);
        var report = await mediator.Send(new GenerateReportQuery(tickerWeights, riskFreeRate));
        return Results.Ok(report);
    }
    catch (FileNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (System.OverflowException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
