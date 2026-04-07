using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Data
{
    public class BookShopContext : DbContext
    {

        public BookShopContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }


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

            modelBuilder.Entity<Customer>(e => {
            e.ToTable("Customers");
                e.HasKey(k => k.Id);
                e.HasMany(o => o.Orders).WithOne(c => c.Customer);               

            });

            modelBuilder.Entity<Genre>(e => {
                e.ToTable("Genre");
                e.HasKey(k => k.Id);
                e.HasMany(b => b.Books).WithMany(g => g.Genres);

            });
            modelBuilder.Entity<Order>(e => {
            e.ToTable("Orders");
                e.HasKey(k => k.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasMany(oi => oi.Items).WithOne(o => o.Order);


            });

            modelBuilder.Entity<Review>(e => {
            e.ToTable("Reviews");
                e.HasKey(k => k.Id);
                e.HasOne(b => b.Book).WithMany(r => r.Reviews);
                e.HasOne(c => c.Customer).WithMany(r => r.Reviews);

            });

            modelBuilder.Entity<OrderItems>(e => {
            e.ToTable("OrderItems");
                e.HasKey(i => new { i.BookId, i.OrderId });
                e.HasOne(o => o.Order).WithMany(i => i.Items);
                e.HasOne(b => b.Book);
            });
        }
    }


}
