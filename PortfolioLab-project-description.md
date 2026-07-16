# PortfolioLab

**A portfolio risk analytics and backtesting platform, built in .NET Clean Architecture.**

Given a set of stock tickers and portfolio weights, PortfolioLab tells you how that portfolio has historically performed and how risky it's been — and given a simple trading rule, how that rule would have performed against real market history.

---

## Why this project exists

This project was built to close a specific, named gap in a job application: a hiring team asked for demonstrated experience with enterprise-grade C#/.NET architectural patterns, hands-on Azure deployment with CI/CD, and a full-stack project showing end-to-end ownership. PortfolioLab is a direct answer to that brief.

It also intentionally sits at the intersection of a Data Science background and backend engineering experience — applying statistics and time-series analysis to a real domain, in a project structured well enough to survive contact with a code review.

---

## What it does

- Ingests historical daily price data (OHLCV) for a fixed universe of tickers into PostgreSQL, refreshed nightly by a background job
- Given a portfolio (tickers + weights), computes:
  - Annualised return
  - Annualised volatility
  - Sharpe ratio
  - Maximum drawdown
  - Historical Value at Risk (95%)
- Runs a moving-average crossover backtest and produces an equity curve, compared against buy-and-hold
- Serves all of the above through a documented REST API
- Visualises portfolio inputs, computed metrics, and the equity curve on a Next.js dashboard

---

## Tech stack

| Concern | Choice |
|---|---|
| Backend framework | ASP.NET Core Web API (C#) |
| Persistence | PostgreSQL via EF Core |
| Caching | Redis *(stretch goal)* |
| Application logic | MediatR (CQRS), FluentValidation |
| Frontend | Next.js |
| Market data source | Stooq (free historical OHLCV, no API key) |
| Background processing | Hosted `BackgroundService`, nightly price refresh |
| Local development | Docker Compose (Postgres + Redis + API) |
| Cloud hosting | Azure App Service (API) + Azure Static Web Apps (frontend) + Azure Database for PostgreSQL |
| CI/CD | GitHub Actions — build, test, migrate, deploy on push to `main` |
| Testing | xUnit, focused on the `Application` layer |

---

## Architecture: Clean Architecture

The system is organised by dependency direction, not technical role. `Domain` has zero outward dependencies — it doesn't know Postgres, Redis, or ASP.NET Core exist. Everything else depends inward.

```
        ┌─────────────────────────┐
        │   Next.js frontend      │
        └────────────┬────────────┘
                      ▼
        ┌─────────────────────────┐
        │      Presentation       │   ASP.NET Core API, Swagger
        └────────────┬────────────┘
                      ▼
        ┌─────────────────────────┐
        │      Application        │   CQRS handlers (MediatR),
        │                         │   risk metrics, backtest engine
        └────────────┬────────────┘
                      ▼
        ┌─────────────────────────┐
        │        Domain           │   Instrument, Portfolio,
        │                         │   PriceBar — pure C#, no I/O
        └─────────────────────────┘

        ┌─────────────────────────┐
        │      Infrastructure     │   EF Core, Postgres, Redis,
        │                         │   Stooq client, background jobs
        └─────────────────────────┘
```

| Layer | Responsibility | Key classes |
|---|---|---|
| **Domain** | Entities and pure business rules. No framework references. | `Instrument`, `Portfolio`, `PortfolioHolding`, `PriceBar`; return/volatility/Sharpe/drawdown/VaR as pure functions |
| **Application** | Use-case orchestration via CQRS. | `GetPortfolioMetricsQuery`, `RunBacktestCommand`, `ImportPriceDataCommand` handlers; FluentValidation |
| **Infrastructure** | Everything that talks to the outside world. | EF Core `DbContext` + repositories, Redis cache decorator, Stooq HTTP client, nightly refresh `BackgroundService` |
| **Presentation** | HTTP boundary. | Controllers/minimal APIs, Swagger/OpenAPI, global exception handling |

---

## Project structure

```
PortfolioLab/
├── src/
│   ├── PortfolioLab.Domain/
│   │   ├── Entities/            (Instrument, Portfolio, PortfolioHolding, PriceBar)
│   │   └── Metrics/             (Return, Volatility, Sharpe, MaxDrawdown, VaR)
│   ├── PortfolioLab.Application/
│   │   ├── Portfolios/          (Queries, Commands, Handlers, Validators)
│   │   └── Backtesting/         (Backtest engine, MA crossover logic)
│   ├── PortfolioLab.Infrastructure/
│   │   ├── Persistence/         (DbContext, EF configurations, migrations)
│   │   ├── Caching/             (Redis decorator)
│   │   ├── MarketData/          (Stooq client, price import)
│   │   └── BackgroundJobs/      (Nightly refresh service)
│   └── PortfolioLab.Api/
│       ├── Controllers/
│       └── Program.cs
├── tests/
│   └── PortfolioLab.Application.Tests/
├── frontend/                    (Next.js dashboard)
├── docker-compose.yml
└── .github/workflows/ci-cd.yml
```

---

## Feature scope

### MVP
- [ ] Price data ingestion (Stooq → Postgres)
- [ ] Nightly background refresh job
- [ ] Return, volatility, Sharpe ratio, max drawdown, historical VaR
- [ ] Moving-average crossover backtest engine
- [ ] REST API with Swagger, validation, global error handling
- [ ] Next.js dashboard: portfolio inputs, metrics table, equity curve chart
- [ ] Azure deployment + GitHub Actions CI/CD

### Stretch goals
- [ ] Redis caching for computed metrics
- [ ] Black–Scholes options pricing calculator
- [ ] Efficient frontier / portfolio optimisation (Modern Portfolio Theory)

*If time runs short, cut stretch goals first — the Azure deployment and CI/CD pipeline should never be cut, as it directly addresses the feedback that motivated this project.*

---

## Build plan (14 days)

| Days | Focus |
|---|---|
| 1–2 | Prototype the five core formulas as plain C# functions against a hardcoded sample series — no database, no API |
| 3–5 | Domain entities, EF Core + Postgres migrations, Docker Compose, price import job |
| 6–8 | CQRS handlers, Presentation API layer, xUnit tests on the Application layer |
| 9–11 | Next.js dashboard: inputs, metrics table, equity curve |
| 12–14 | Azure deployment, GitHub Actions CI/CD, README polish, buffer |

---

## Core formulas reference

```
Simple return          R_t = (P_t − P_(t−1)) / P_(t−1)
Log return              r_t = ln(P_t / P_(t−1))

Annualised return       ≈ mean(daily return) × 252
Annualised volatility   = stdev(daily return) × √252

Sharpe ratio            = (annualised return − risk-free rate) / annualised volatility

Max drawdown            Drawdown_t = (Value_t − RunningMax_t) / RunningMax_t
                         MaxDrawdown = min(Drawdown_t)

Historical VaR (95%)    = 5th percentile of historical daily return distribution

MA crossover rule       Enter when ShortMA crosses above LongMA
                         Exit when ShortMA crosses back below LongMA
```

---

## API endpoints (draft)

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/instruments` | List available tickers |
| `POST` | `/api/portfolios/metrics` | Compute risk/performance metrics for a given portfolio |
| `POST` | `/api/backtests/ma-crossover` | Run a moving-average crossover backtest |
| `GET` | `/api/instruments/{ticker}/prices` | Historical price history for a ticker |

---

## The pitch, in one breath

> Built a portfolio analytics engine in Clean Architecture .NET, with EF Core/Postgres for persistence, MediatR-based CQRS for use cases, and a background service refreshing market data — deployed to Azure with a GitHub Actions pipeline. The risk maths sits in the Domain and Application layers as pure, fully unit-tested C#, with zero dependency on the database or the web framework.

---

## License

MIT
