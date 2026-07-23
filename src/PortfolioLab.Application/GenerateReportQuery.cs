using MediatR;

namespace PortfolioLab.Application;

public record GenerateReportQuery(Dictionary<string, double> TickerWeights, double RiskFreeRate):IRequest<PortfolioReport>;