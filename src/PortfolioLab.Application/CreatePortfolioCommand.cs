using MediatR;

namespace PortfolioLab.Application;

public record CreatePortfolioCommand(string Name, Dictionary<string, double> TickerWeights): IRequest<int>;