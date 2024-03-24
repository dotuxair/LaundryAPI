﻿// <auto-generated />
using System;
using FYP.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FYP.API.Migrations
{
    [DbContext(typeof(LaundaryDbContext))]
    [Migration("20240320060350_RetailerAdd")]
    partial class RetailerAdd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
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

                    b.Property<DateOnly>("BookingDate")
                        .HasColumnType("date");

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("float");

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

                    b.Property<int?>("LaundryMachineId")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("LaundryMachineId");

                    b.ToTable("BookingMachines");
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

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("RetailerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("RetailerId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryMachine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("LoadCapacity")
                        .HasColumnType("int");

                    b.Property<string>("MachineCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MachineType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int?>("RetailerId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("RetailerId");

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

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Percentage")
                        .HasColumnType("int");

                    b.Property<int>("PriceLimit")
                        .HasColumnType("int");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("ProgramId");

                    b.ToTable("Offers");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Program", b =>
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

                    b.Property<string>("SpinSpeed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Temprature")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.PurchasedItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<int?>("LaundryItemId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("LaundryItemId");

                    b.ToTable("PurchasedProducts");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Retailer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Retailers");
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
                            Email = "dotuxair@gmail.com",
                            Name = "Uxair Ijaz",
                            Password = "abc@123",
                            PhoneNumber = "0345-6756919"
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
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("FYP.API.Models.Domain.LaundryMachine", "LaundryMachine")
                        .WithMany()
                        .HasForeignKey("LaundryMachineId");

                    b.Navigation("Booking");

                    b.Navigation("LaundryMachine");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Branch", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany("Branches")
                        .HasForeignKey("AdminId");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryItem", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("FYP.API.Models.Domain.Retailer", "Retailer")
                        .WithMany()
                        .HasForeignKey("RetailerId");

                    b.Navigation("Branch");

                    b.Navigation("Retailer");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.LaundryMachine", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP.API.Models.Domain.Retailer", "Retailer")
                        .WithMany()
                        .HasForeignKey("RetailerId");

                    b.Navigation("Branch");

                    b.Navigation("Retailer");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Offer", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.HasOne("FYP.API.Models.Domain.Program", "Program")
                        .WithMany()
                        .HasForeignKey("ProgramId");

                    b.Navigation("Admin");

                    b.Navigation("Program");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Program", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.PurchasedItem", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("FYP.API.Models.Domain.LaundryItem", "LaundryItem")
                        .WithMany()
                        .HasForeignKey("LaundryItemId");

                    b.Navigation("Booking");

                    b.Navigation("LaundryItem");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Retailer", b =>
                {
                    b.HasOne("FYP.API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP.API.Models.Domain.Admin", b =>
                {
                    b.Navigation("Branches");
                });
#pragma warning restore 612, 618
        }
    }
}
