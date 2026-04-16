using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using BooksRepository = Infrastructure.Repositories.BooksRepository;


namespace Application
{
   public static  class DI {
        public static IServiceCollection Addifrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContextPool<BookShopContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBooksRepository,BooksRepository>();
            services.AddScoped<IAuthorsRepository, AuthorsRepository>();
            services.AddScoped<ICustomersRepository,CustomersRepository>();
            services.AddScoped<IGenresRepository,GenresRepository>();
            services.AddScoped<IOrdersRepository,OrdersRepository>();
            services.AddScoped<IReviewsRepository,ReviewsRepository>();
            services.AddScoped<IOrderItemsRepository,OrderItemsRepository>();
            services.AddScoped<IBookService,BookService>();
            services.AddScoped<IAuthorService,AuthorService>();
            services.AddScoped<IGenreService,GenreService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRatingService, RatingService>();
        /*    services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
        */
            services.AddHybridCache();
            return services;
        
        }

    }
}
