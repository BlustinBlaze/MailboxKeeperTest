using System.Text.Json;
using API;
using Application.DTOs.Order;
using Application.Entities.Stats;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(ApiContext db, IMapper mapper, ILogger<OrderRepository> logger) : IOrdersRepository
{
    private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1);
    public void CreateOrder(CustomerDto customerDto, OrderDto orderDto)
    {
        var customer = CreateCustomer(customerDto);
        var order = CreateOrder(orderDto, customer.CustomerId);
        CreateOrderItems(orderDto, order.OrderId);
        
        UpdateOrderStats(orderDto);
        logger.LogInformation("Order created successfully.");
    }
    
    private Customer CreateCustomer(CustomerDto customerDto)
    {
        logger.LogInformation("Adding customer to database...");
        var customer = mapper.Map<Customer>(customerDto);
        //db.Customers.Add(customer);
        db.SaveChanges();
        logger.LogInformation("Customer added to database successfully.");
        return customer;
    }
    
    private Order CreateOrder(OrderDto orderDto, int customerId)
    {
        logger.LogInformation("Adding order to database...");
        var order = mapper.Map<Order>(orderDto);
        order.CustomerId = customerId;
        //db.Orders.Add(order);
        db.SaveChanges();
        logger.LogInformation("Order added to database successfully.");
        return order;
    }
    
    private void CreateOrderItems(OrderDto orderDto, int orderId)
    {
        logger.LogInformation("Adding order items to database...");
        foreach (var orderItem in orderDto.Items)
        {
            var item = mapper.Map<OrderItem>(orderItem);
            item.OrderId = orderId;
            //db.OrderItems.Add(item);
            db.SaveChanges();
            logger.LogInformation("Order item added to database successfully.");
        }
    }

    private void UpdateOrderStats(OrderDto order)
    {
        logger.LogInformation("Updating order stats...");
        const string fileName = "order_stats.json";
        try
        {
            FileLock.Wait();
            OrderStats stats;
            if (!File.Exists(fileName) || string.IsNullOrEmpty(File.ReadAllText(fileName)))
            {
                stats = CreateNewOrderStats(order);
            }
            else
            {
                stats = UpdateExistingOrderStats(order, fileName);
            }
            WriteOrderStatsToFile(stats, fileName);
            logger.LogInformation("Order stats updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating order stats.");
            throw;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private OrderStats CreateNewOrderStats(OrderDto order)
    {
        logger.LogInformation("Creating new order stats file...");
        var stats = new OrderStats
        {
            Stats = new Stats
            {
                Orders = 1,
                OrderItems = order.Items.Count
            },
            Country = new CountryStats
            {
                Canada = order.Country.ToLower() == "canada" ? 1 : 0,
                Other = order.Country.ToLower() != "canada" ? 1 : 0
            }
        };
        logger.LogInformation("Order stats created successfully.");
        return stats;
    }

    private OrderStats UpdateExistingOrderStats(OrderDto order, string fileName)
    {
        logger.LogInformation("Updating existing order stats...");
        var orderStats = File.ReadAllText(fileName);
        var stats = JsonSerializer.Deserialize<OrderStats>(orderStats);
        if (stats == null)
        {
            throw new InvalidOperationException("Failed to read order stats from file");
        }
        stats.Stats.Orders++;
        stats.Stats.OrderItems += order.Items.Count;
        if (order.Country.ToLower() == "canada")
        {
            stats.Country.Canada += 1;
        }
        else
        {
            stats.Country.Other += 1;
        }
        return stats;
    }

    private void WriteOrderStatsToFile(OrderStats stats, string fileName)
    {
        var orderStats = JsonSerializer.Serialize(stats);
        File.WriteAllText(fileName, orderStats);
    }
}