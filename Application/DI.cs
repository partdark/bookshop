using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;




namespace Application
{
   public static  class DI {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) 
        {

           
            services.AddScoped<IBookService,BookService>();
            services.AddScoped<IAuthorService,AuthorService>();
            services.AddScoped<IGenreService,GenreService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddHostedService<RatingRecalculationStartupService>();
            /*    services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                });
            */
            services.AddHybridCache();

            services.AddInfrastructureServices(configuration);
            return services;
        
        }

    }
}
