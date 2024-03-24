﻿using FYP.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace FYP.API.Data
{
    public class LaundaryDbContext : DbContext
    {
        public LaundaryDbContext(DbContextOptions<LaundaryDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PurchasedItem> PurchasedProducts { get; set; }
        public DbSet<BookingDetail> BookingMachines { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<LaundryMachine> Machines { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<LaundryItem> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Retailer> Retailers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Admin>()
         .HasMany(a => a.Branches)
         .WithOne(a => a.Admin)
         .HasForeignKey(b => b.AdminId);


            modelBuilder.Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        Name = "Uxair Ijaz",
                        Email = "dotuxair@gmail.com",
                        Password = "abc@123",
                        PhoneNumber = "0345-6756919"
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
