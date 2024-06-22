namespace FYP.API.Models.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date { get; set; }
        public double Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string branchName { get; set; } = string.Empty;
        public string? programName { get; set; }

    }
}
