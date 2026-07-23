using MediatR;

namespace PortfolioLab.Application;

public record CreatePortfolioCommand(string Name, Dictionary<string, double> TickerWeights): IRequest<int>
{
    public string UserId {get;set;} = string.Empty;
}