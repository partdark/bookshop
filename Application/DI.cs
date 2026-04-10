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
            services.AddDbContext<BookShopContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBooksRepository,BooksRepository>();
            services.AddScoped<IAuthorsRepository, AuthorsRepository>();
            services.AddScoped<ICustomersRepository,CustomersRepository>();
            services.AddScoped<IGenresRepository,GenresRepository>();
            services.AddScoped<IOrdersRepository,OrdersRepository>();
            services.AddScoped<IReviewsRepository,ReviewsRepository>();
            services.AddScoped<IOrderItemsRepository,OrderItemsRepository>();
            services.AddScoped<IBookService,BookService>();
            services.AddScoped<IAuthorService,AuthorService>();
            return services;
        
        }

    }
}
