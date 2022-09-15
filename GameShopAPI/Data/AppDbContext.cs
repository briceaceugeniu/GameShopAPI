using GameShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Address> Address => Set<Address>();
        public DbSet<CartItem> CartItem => Set<CartItem>();
        public DbSet<CreditCard> CreditCard => Set<CreditCard>();
        public DbSet<Customer> Customer => Set<Customer>();
        public DbSet<Developer> Developer => Set<Developer>();
        public DbSet<Game> Game => Set<Game>();
        public DbSet<GameCategory> GameCategory => Set<GameCategory>();
        public DbSet<Order> Order => Set<Order>();
        public DbSet<OrderItem> OrderItem => Set<OrderItem>();
        public DbSet<PaymentInfo> PaymentInfo => Set<PaymentInfo>();
        public DbSet<Publisher> Publisher => Set<Publisher>();
        public DbSet<Role> Role => Set<Role>();
    }
}
