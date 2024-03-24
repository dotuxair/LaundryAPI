﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Booking
    {
        public int Id { get; set; }

        public DateOnly BookingDate { get; set; }

        public double TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;


        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        public User? User { get; set; }
        public int? UserId { get; set; }

    }
}