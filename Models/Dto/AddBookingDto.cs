using FYP.API.Models.Domain;

namespace FYP.API.Models.Dto
{
    public class AddBookingDto
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;

        public string WashCycle { get; set; } = string.Empty;
        public string DryCycle { get; set; } = string.Empty;

        public int TotalPrice { get; set; }
        public List<ProductsData>? Products { get; set; }
        public int BranchId { get; set; }
        public List<MachineIds>? Machines { get; set; }

    }
    public class ProductsData
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class MachineIds
    {
        public string Capacity { get; set; } = string.Empty;
    }
}
