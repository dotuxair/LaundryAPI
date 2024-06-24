namespace FYP.API.Models.Dto
{
    public class BulkClothRequestDto
    {
        public string RequestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int BranchId { get; set; }
        public DateTime PickUpDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
