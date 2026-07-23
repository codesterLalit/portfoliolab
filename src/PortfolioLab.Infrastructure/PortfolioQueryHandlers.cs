using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioLab.Application;

namespace PortfolioLab.Infrastructure;

public class GetPortfolioHandler: IRequestHandler<GetPortfolioQuery, SavedPortfolioDto?>
{
    private readonly PriceDataDbContext _db;
    public GetPortfolioHandler(PriceDataDbContext db) => _db = db;

    public async Task<SavedPortfolioDto?> Handle(GetPortfolioQuery request, CancellationToken ct)
    {
        var entity = await _db.SavedPortfolios.FindAsync([request.Id]);
        if (entity is null) return null;

        var weights = JsonSerializer.Deserialize<Dictionary<string, double>>(entity.TickerWeightsJson)!;
        return new SavedPortfolioDto(entity.Id, entity.Name, weights);
    }
}

public class ListPortfoliosHandler: IRequestHandler<ListPortfoliosQuery, List<SavedPortfolioDto>>
{
    private readonly PriceDataDbContext _db;
    public ListPortfoliosHandler(PriceDataDbContext db) => _db = db;

    public async Task<List<SavedPortfolioDto>> Handle(ListPortfoliosQuery request, CancellationToken ct)
    {
        var entities = await _db.SavedPortfolios.ToListAsync(ct);
        return entities.Select(e => new SavedPortfolioDto(
            e.Id, e.Name, JsonSerializer.Deserialize<Dictionary<string, double>>(e.TickerWeightsJson)!
        )).ToList();
    }
}