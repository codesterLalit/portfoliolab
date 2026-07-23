namespace PortfolioLab.Infrastructure;

public class PriceBarEntity
{
    public int Id {get; set;}
    public string Ticker {get; set; } = string.Empty;
    public DateOnly Date {get; set;}
    public double Close {get; set;}
}