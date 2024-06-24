using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double Price { get; set; }


        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        public User? User { get; set; }
        public int? UserId { get; set; }

        public ICollection<BookingDetail>? BookingDetails { get; set; }
    }
}
