using MediatR;

namespace PortfolioLab.Application;

public class GenerateReportHandler: IRequestHandler<GenerateReportQuery, PortfolioReport>
{
    private readonly PortfolioAnalysisService _service;

    public GenerateReportHandler(PortfolioAnalysisService service)
    {
        _service = service;
    }

    public Task<PortfolioReport> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
    {
        var report = _service.GenerateReport(request.TickerWeights, request.RiskFreeRate);
        return Task.FromResult(report);
    }
}