namespace Application.DTOs.Order;

public class CreateOrderRequest
{
    public CustomerDto Customer { get; set; }
    public OrderDto Order { get; set; }
}