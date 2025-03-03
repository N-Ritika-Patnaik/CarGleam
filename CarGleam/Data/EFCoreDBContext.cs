using CarGleam.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGleam.Data
{
    // this is db context class
    // EFCoreDbContext class inherits from DbContext, which is the primary class for interacting with the database using EF Core.
    public class EFCoreDBContext : DbContext
    {
        // Constructor that accepts DbContext class constructor (DbContextOptions<EFCoreDbContext>) as a parameter.
        // The options parameter contains the settings required by EF Core to configure the DbContext, such as the connection string and provider.
        public EFCoreDBContext(DbContextOptions<EFCoreDBContext> options) : base(options) { }
        // The base(options) call passes the options to the base DbContext class constructor.

        public DbSet<User> Users { get; set; } // has all attribures of User class
        public DbSet<ServiceLocation> ServiceLocations { get; set; } //represents a table in the database corresponding to the ServiceLocation entity.
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings) // User has many Bookings
                .WithOne(b => b.User) // Booking has one User
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); // if user is deleted, all bookings related to that user will also be deleted

            modelBuilder.Entity<ServiceLocation>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.ServiceLocation)
                .HasForeignKey(b => b.ServiceLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Machine>()
                .HasMany(m => m.Bookings)
                .WithOne(b => b.Machine)
                .HasForeignKey(b => b.MachineId)
                .OnDelete(DeleteBehavior.SetNull);

            // upar 3 ka reverse hai ye i.e booking se kese related hai user, servicelocation, machine
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User) // Booking has one User
                .WithMany(u => u.Bookings) // User has many Bookings
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServiceLocation)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Machine)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MachineId)
                .OnDelete(DeleteBehavior.SetNull);
            //------------------------------------------------------

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Booking) // Transaction has one Booking
                .WithMany(b => b.Transactions) // Booking has many Transactions
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceLocation>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.PaymentAmount)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
