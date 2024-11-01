using Application.DTOs.Order;

namespace Application.Interfaces;

public interface IOrdersRepository
{
    void CreateOrder(CustomerDto customerDto, OrderDto orderDto);
}