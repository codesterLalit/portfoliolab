using MediatR;

namespace PortfolioLab.Application;

public record DeletePortfolioCommand(int Id, string UserId): IRequest<bool>;