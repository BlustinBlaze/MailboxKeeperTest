namespace Application.DTOs.Stats;

public class OrderStatsResponse
{
    public int OrdersCount { get; set; }
    public int OrderItemsCount { get; set; }
    public int OrderedFromCanadaCount { get; set; }
    public int OrderedFromOtherCountriesCount { get; set; }
}