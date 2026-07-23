namespace PortfolioLab.Infrastructure;

public class SavedPortfolioEntity
{
    public int Id {get; set;}
    public string Name {get; set; } = string.Empty;
    public string TickerWeightsJson {get; set; } = string.Empty;
}