using System;
using System.Collections.Generic;
using System.Text;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<PaymentType> PaymentType { get; set; }
        
        public DbSet<ProductType> Products { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.Entity<Order> ()
                .Property (b => b.DateCreated)
                .HasDefaultValueSql ("GETDATE()");

            // Restrict deletion of related order when OrderProducts entry is removed
            modelBuilder.Entity<Order> ()
                .HasMany (o => o.OrderProducts)
                .WithOne (l => l.Order)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Product> ()
                .Property (b => b.DateCreated)
                .HasDefaultValueSql ("GETDATE()");

            // Restrict deletion of related product when OrderProducts entry is removed
            modelBuilder.Entity<Product>()
                .HasMany(o => o.OrderProducts)
                .WithOne(l => l.Product)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentType> ()
                .Property (b => b.DateCreated)
                .HasDefaultValueSql ("GETDATE()");
                
            ApplicationUser user = new ApplicationUser {
                FirstName = "Admina",
                LastName = "Straytor",
                StreetAddress = "123 Infinity Way",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "67abd6d3-7436-4448-8aec-08d8a5641178", 
                Id = "3abf5f13-de66-480c-adad-8d1e1eecf318"
            };
            var passwordHash = new PasswordHasher<ApplicationUser> ();
            user.PasswordHash = passwordHash.HashPassword (user, "P8ssword!");
            modelBuilder.Entity<ApplicationUser> ().HasData (user);

            modelBuilder.Entity<PaymentType> ().HasData (
                new PaymentType () {
                    PaymentTypeId = 1,
                        UserId = user.Id,
                        Description = "American Express",
                        AccountNumber = "86753095551212"
                },
                new PaymentType () {
                    PaymentTypeId = 2,
                        UserId = user.Id,
                        Description = "Discover",
                        AccountNumber = "4102948572991"
                }
            );

            modelBuilder.Entity<ProductType> ().HasData (
                new ProductType () {
                    ProductTypeId = 1,
                        Label = "Sporting Goods"
                },
                new ProductType () {
                    ProductTypeId = 2,
                        Label = "Appliances"
                }
            );

            modelBuilder.Entity<Product> ().HasData (
                new Product () {
                    ProductId = 1,
                        ProductTypeId = 1,
                        UserId = user.Id,
                        Description = "It flies high",
                        Title = "Kite",
                        Quantity = 100,
                        Price = 2.99
                },
                new Product () {
                    ProductId = 2,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "It rolls fast",
                    Title = "Wheelbarrow",
                    Quantity = 5,
                    Price = 29.99
                },
                new Product()
                {
                    ProductId = 3,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Really blends it well",
                    Title = "Blender",
                    Quantity = 2,
                    Price = 19.99
                },
                new Product()
                {
                    ProductId = 4,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Sharp",
                    Title = "Knife Set",
                    Quantity = 1,
                    Price = 59.99
                },
                new Product()
                {
                    ProductId = 5,
                    ProductTypeId = 2,
                    UserId = user.Id,
                    Description = "Black and Sleek",
                    Title = "Microwave",
                    Quantity = 2,
                    Price = 100.00
                }
            );

            modelBuilder.Entity<Order> ().HasData (
                new Order () {
                    OrderId = 1,
                    UserId = user.Id,
                    PaymentTypeId = null
                },
                new Order ()
                {
                    OrderId = 2,
                    UserId = user.Id,
                    PaymentTypeId = null
                }

            );

            modelBuilder.Entity<OrderProduct> ().HasData (
                new OrderProduct () {
                    OrderProductId = 1,
                    OrderId = 1,
                    ProductId = 1
                }
            );

            modelBuilder.Entity<OrderProduct> ().HasData (
                new OrderProduct () {
                    OrderProductId = 2,
                    OrderId = 1,
                    ProductId = 2
                }
            );

        }
    }
}