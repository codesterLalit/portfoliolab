using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioLab.Api;
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(GenerateReportQuery).Assembly,
    typeof(CreatePortfolioHandler).Assembly
));

builder.Services.AddScoped<PortfolioAnalysisService>();



builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;    
})
    .AddEntityFrameworkStores<PriceDataDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = "this-is-a-dev-only-secret-key-change-me-1234567890";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
       options.TokenValidationParameters = new TokenValidationParameters
       {
         ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
       }; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAuthEndpoints(jwtKey);

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

app.MapPost("/portfolios", async(CreatePortfolioCommand cmd, IMediator mediator, HttpContext ctx) =>{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
    var id = await mediator.Send(cmd with {UserId = userId});
    return Results.Created($"/portfolios/{id}", new {id});    
}).RequireAuthorization();

app.MapGet("/portfolios/{id:int}", async (int id, IMediator mediator, HttpContext ctx) =>
{
   var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
   var portfolio = await mediator.Send(new GetPortfolioQuery(id, userId));
   return portfolio is null? Results.NotFound() : Results.Ok(portfolio); 
}).RequireAuthorization();

app.MapGet("/portfolios", async(IMediator mediator, HttpContext ctx) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
   return Results.Ok(await mediator.Send(new ListPortfoliosQuery(userId))); 
}).RequireAuthorization();

app.MapDelete("/portfolios/{id:int}", async(int id, IMediator mediator, HttpContext ctx) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
   var deleted = await mediator.Send(new DeletePortfolioCommand(id, userId));
   return deleted ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
