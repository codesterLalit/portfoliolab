using MediatR;

namespace PortfolioLab.Application;

public record DeletePortfolioCommand(int Id): IRequest<bool>;