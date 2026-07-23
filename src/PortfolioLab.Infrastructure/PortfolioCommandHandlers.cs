using System.Text.Json;
using MediatR;
using PortfolioLab.Application;

namespace PortfolioLab.Infrastructure;

public class CreatePortfolioHandler: IRequestHandler<CreatePortfolioCommand, int>
{
    private readonly PriceDataDbContext _db;
    public CreatePortfolioHandler(PriceDataDbContext db) => _db = db;

    public async Task<int> Handle(CreatePortfolioCommand request, CancellationToken ct)
    {
        var entity = new SavedPortfolioEntity
        {
          Name = request.Name,
          TickerWeightsJson = JsonSerializer.Serialize(request.TickerWeights)  
        };
        _db.SavedPortfolios.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public class DeletePortfolioHandler: IRequestHandler<DeletePortfolioCommand, bool>
    {
        private readonly PriceDataDbContext _db;
        public DeletePortfolioHandler(PriceDataDbContext db) => _db = db;

        public async Task<bool> Handle(DeletePortfolioCommand request, CancellationToken ct)
        {
            var entity = await _db.SavedPortfolios.FindAsync([request.Id]);
            if (entity is null) return false;
            _db.SavedPortfolios.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}