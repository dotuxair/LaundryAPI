namespace FYP.API.Models.Dto
{
    public class OfferDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int OffPercentage { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
