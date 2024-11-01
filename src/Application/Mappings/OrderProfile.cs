using API;
using Application.DTOs.Order;
using AutoMapper;

namespace Application.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<CustomerDto, Customer>();
        CreateMap<OrderDto, Order>();
        CreateMap<OrderItemDto, OrderItem>();

        CreateMap<Customer, CustomerDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}