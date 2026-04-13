using Application;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSwaggerGen();
//builder.Services.AddDbContext<BookShopContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Addifrastructure(builder.Configuration);

builder.Services.AddIdentity<Customer, IdentityRole<Guid>>(
    options => {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        }
    ).AddEntityFrameworkStores<BookShopContext>().AddDefaultTokenProviders();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  //  var context = scope.ServiceProvider.GetRequiredService<BookShopContext>();
  //  context.Database.Migrate();
  var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BookShopContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

       await context.Database.MigrateAsync();

        var roles = new[] { "Admin", "user" };
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
