namespace FYP.API.Models.Domain
{
    public class Review
    {
        public int Id { get; set; }
        public int Stars { get; set; }
        public string   UserThoughts { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
