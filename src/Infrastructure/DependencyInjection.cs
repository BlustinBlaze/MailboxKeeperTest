using API;
using Application.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApiContext>(
            options => options.UseNpgsql("Name=ConnectionString:Api"));
        
        services.AddScoped<IOrdersRepository, OrderRepository>();
        services.AddScoped<IStatsRepository, StatsRepository>();
        services.AddScoped<IMailboxRepository, MailboxRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}