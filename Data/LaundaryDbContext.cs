using FYP.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FYP.API.Data
{
    public class LaundaryDbContext : DbContext
    {
        public LaundaryDbContext(DbContextOptions<LaundaryDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PurchasedProduct> PurchasedProducts { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BranchManager> BranchManagers { get; set; }
        public DbSet<LoadCapacity> LoadCapacity { get; set; }
        public DbSet<BulkCloth> BulkClothes { get; set; }
        public DbSet<LaundryProgram> LaundryPrograms { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BulkCloth>()
      .Property(b => b.PriceOffered)
      .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BulkCloth>()
  .Property(b => b.AcceptedPrice)
  .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Admin>()
         .HasMany(a => a.Branches)
         .WithOne(a => a.Admin)
         .HasForeignKey(b => b.AdminId);

            modelBuilder.Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        Name = "Admin",
                        Email = "admin@gmail.com",
                        Password = "abc@123",
                        PhoneNumber = "0000-0000000"
                    }
                );

            modelBuilder.Entity<Admin>()
                .HasData(
                    new Admin
                    {
                        Id = 1,
                        UserId = 1,
                    }
                );
         
            base.OnModelCreating(modelBuilder);
        }
    }
}
