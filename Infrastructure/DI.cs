using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContextPool<BookShopContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IBooksRepository, BooksRepository>();
            services.AddScoped<IAuthorsRepository, AuthorsRepository>();
            services.AddScoped<ICustomersRepository, CustomersRepository>();
            services.AddScoped<IGenresRepository, GenresRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IReviewsRepository, ReviewsRepository>();
            services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddIdentity<Customer, IdentityRole<Guid>>(options =>
             {
                 options.SignIn.RequireConfirmedAccount = false;
                 options.Password.RequireDigit = false;
                 options.Password.RequiredLength = 4;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;
             }).AddEntityFrameworkStores<BookShopContext>().AddDefaultTokenProviders();

            /*    services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                });
            */
            return services;

        }

    }
}

