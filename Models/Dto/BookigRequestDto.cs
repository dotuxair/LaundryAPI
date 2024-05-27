namespace FYP.API.Models.Dto
{
    public class BookingData
    {
        public int BranchId { get; set; }
        public int LaundryIntervals { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public double PriceAfterDiscount { get; set; }
        public double TotalPrice { get; set; }
        public List<AvailableMachines>? Machines { get; set; }
        public List<SelectedItemsList>? Items { get; set; }
    }
}
