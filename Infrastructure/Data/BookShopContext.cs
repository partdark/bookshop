using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Data
{
    public class BookShopContext : IdentityDbContext<Customer, IdentityRole<Guid> , Guid>
    {

        public BookShopContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Author> Authors { get; set; }


        //Используется core identity user
        //  public DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<OrderItems> OrderItems { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(e =>
            {
                e.ToTable("Books");
                e.HasKey(k => k.Id);
                e.HasMany(a => a.Authors).WithMany(b => b.Books);
                e.HasMany(r => r.Reviews).WithOne(b => b.Book);
                e.HasMany(g => g.Genres).WithMany(b => b.Books);

            });

            modelBuilder.Entity<Author>(e =>
            {
                e.ToTable("Authors");
                e.HasKey(k => k.Id);
                e.HasMany(a => a.Books).WithMany(b => b.Authors);
            });

            //Используется core identity user
            //modelBuilder.Entity<Customer>(e =>
            //{
            //    e.ToTable("Customers");
            //    e.HasKey(k => k.Id);
            //    e.HasIndex(c => c.Mail).IsUnique();
            //    e.HasIndex(c => c.Name).IsUnique();
            //    e.HasMany(o => o.Orders).WithOne(c => c.Customer);

            //});

            modelBuilder.Entity<Genre>(e =>
            {
                e.ToTable("Genre");
                e.HasKey(k => k.Id);
                e.HasMany(b => b.Books).WithMany(g => g.Genres);

            });
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(k => k.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasMany(oi => oi.Items).WithOne(o => o.Order);
                e.HasOne(c => c.Customer).WithMany(o => o.Orders).HasForeignKey(c => c.CustomerId);


            });

            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("Reviews");
                e.HasKey(k => k.Id);
                e.HasOne(b => b.Book).WithMany(r => r.Reviews);
                e.HasOne(c => c.Customer).WithMany(r => r.Reviews);
                e.HasIndex(r => new { r.CustomerId, r.BookId }).IsUnique();

            });

            modelBuilder.Entity<OrderItems>(e =>
            {
                e.ToTable("OrderItems");
                e.HasKey(i => new { i.BookId, i.OrderId });
                e.HasOne(o => o.Order).WithMany(i => i.Items);
                e.HasOne(b => b.Book);
            });
            modelBuilder.Entity<CartItem>(e => {
                e.ToTable($"CartItems");
                e.HasKey(c => c.Id);
                e.HasOne(c => c.Customer).WithMany(u => u.CartItems);
                e.HasOne(c => c.Book).WithMany().HasForeignKey(c => c.BookId);
                e.HasIndex(c => new { c.CustomerId, c.BookId }).IsUnique();

            });
                        
        }
    }


}

