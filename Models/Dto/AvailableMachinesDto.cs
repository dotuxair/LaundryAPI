namespace FYP.API.Models.Dto
{
    public class AvailableMachinesDto
    {
        public string BranchName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public int BranchId { get; set; }
        public double Price { get; set; }
        public bool AfterBookingSlot { get; set; }
        public bool BeforeBookingSlot { get; set; }
    }
}

