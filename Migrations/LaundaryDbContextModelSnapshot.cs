﻿// <auto-generated />
using System;
using FYP.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FYP.API.Migrations
{
    [DbContext(typeof(LaundaryDbContext))]
    partial class LaundaryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FYP.API.Models.Domain.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Admins");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            UserId = 1
                        });
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BookingDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("Cycles")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<int?>("LaundryProgramId")
                        .HasColumnType("int");

                    b.Property<int?>("MachineId")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("LaundryProgramId");

                    b.HasIndex("MachineId");

                    b.ToTable("BookingDetails");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BranchManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("BranchManagers");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BulkCloth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("AcceptedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateAccepted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRequested")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("InformManager")
                        .HasColumnType("bit");

                    b.Property<bool>("InformUser")
                        .HasColumnType("bit");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PickUpDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PriceOffered")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RequestName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("UserId");

                    b.ToTable("BulkClothes");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryProgram", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("SpinSpeed")
                        .HasColumnType("int");

                    b.Property<string>("Temprature")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("LaundryPrograms");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LoadCapacity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("LoadCapacity");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Machine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("LoadCapacityId")
                        .HasColumnType("int");

                    b.Property<string>("MachineCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("LoadCapacityId");

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Offer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LaundryProgramId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OffPercentage")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("LaundryProgramId");

                    b.ToTable("Offers");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.PurchasedProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("ProductId");

                    b.ToTable("PurchasedProducts");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("Stars")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserThoughts")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("BranchId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@gmail.com",
                            Name = "Admin",
                            Password = "abc@123",
                            PhoneNumber = "0000-0000000"
                        });
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Admin", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Booking", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Branch");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BookingDetail", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Booking", "Booking")
                        .WithMany("BookingDetails")
                        .HasForeignKey("BookingId");

                    b.HasOne("FYP.API.Models.Domain.LaundryProgram", "LaundryProgram")
                        .WithMany()
                        .HasForeignKey("LaundryProgramId");

                    b.HasOne("FYP.API.Models.Domain.Machine", "Machine")
                        .WithMany()
                        .HasForeignKey("MachineId");

                    b.Navigation("Booking");

                    b.Navigation("LaundryProgram");

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Branch", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany("Branches")
                        .HasForeignKey("AdminId");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BranchManager", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithOne("BranchManager")
                        .HasForeignKey("FYP.API.Models.Domain.BranchManager", "BranchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithOne("BranchManager")
                        .HasForeignKey("FYP.API.Models.Domain.BranchManager", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Branch");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.BulkCloth", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryProgram", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LoadCapacity", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Machine", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP.API.Models.Domain.LoadCapacity", "LoadCapacity")
                        .WithMany()
                        .HasForeignKey("LoadCapacityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("LoadCapacity");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Offer", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.HasOne("FYP.API.Models.Domain.LaundryProgram", "LaundryProgram")
                        .WithMany()
                        .HasForeignKey("LaundryProgramId");

                    b.Navigation("Admin");

                    b.Navigation("LaundryProgram");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Product", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.PurchasedProduct", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("FYP.API.Models.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Booking");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Review", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Booking");

                    b.Navigation("Branch");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Admin", b =>
                {
                    b.Navigation("Branches");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Booking", b =>
                {
                    b.Navigation("BookingDetails");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Branch", b =>
                {
                    b.Navigation("BranchManager");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.User", b =>
                {
                    b.Navigation("BranchManager");
                });
#pragma warning restore 612, 618
        }
    }
}
