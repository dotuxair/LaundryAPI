namespace FYP.API.Models.Dto
{
    public class ReviewDto
    {
        public int BookingId { get; set; }
        public int Stars { get; set; }
        public string UserThoughts { get; set; } = string.Empty;
    }
}
