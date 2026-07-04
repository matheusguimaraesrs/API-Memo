using Domain.IRepositories;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Memo.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<ISellerRepository, SellerRepository>();

        return services;
    }
}