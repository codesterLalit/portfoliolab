using MediatR;

namespace PortfolioLab.Application;

public record SavedPortfolioDto(int Id, string Name, Dictionary<string, double> TickerWeights);
public record GetPortfolioQuery(int Id): IRequest<SavedPortfolioDto?>;
public record ListPortfoliosQuery(): IRequest<List<SavedPortfolioDto>>;