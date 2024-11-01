using Application.DTOs.Order;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class OrdersController(IOrdersRepository ordersRepository, ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    public ActionResult Create(CreateOrderRequest request)
    {
        logger.LogInformation("Creating a new order");
        try
        {
            ordersRepository.CreateOrder(request.Customer, request.Order);
            logger.LogInformation("Order created for customer {customer}", request.Customer.Firstname + " " + request.Customer.Lastname);
            return StatusCode(201);
        } catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the order.");
            return StatusCode(500, ex.Message);
        }
        
    }
}