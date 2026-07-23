using MediatR;

namespace PortfolioLab.Application;

public record SavedPortfolioDto(int Id, string Name, Dictionary<string, double> TickerWeights);
public record GetPortfolioQuery(int Id, string UserId): IRequest<SavedPortfolioDto?>;
public record ListPortfoliosQuery(string UserId): IRequest<List<SavedPortfolioDto>>;