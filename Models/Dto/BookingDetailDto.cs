namespace FYP.API.Models.Dto
{
    public class BookingDetailDto
    {
        public int Id { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<PurchasedProducts>? Products { get; set; }
        public List<MachineCycle>? Machines { get; set; }
    }

    public class PurchasedProducts
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

    }

    public class MachineCycle
    {
        public int Id { get; set; }
        public string DryCycle { get; set; } = string.Empty;
        public string WashCycle { get; set; } = string.Empty;
    }
}
