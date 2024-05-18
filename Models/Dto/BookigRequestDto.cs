namespace FYP.API.Models.Dto
{
    public class BookingData
    {
        public int BranchId { get; set; }
        public int Cycles { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<SelectedItemsList>? Items { get; set; }
        public int LoadCapacity { get; set; }
        public int MachineId { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public double PriceAfterDiscount { get; set; }
        public int ProgramId { get; set; }
        public double TotalPrice { get; set; }
    }
    }
