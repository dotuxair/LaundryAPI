namespace FYP.API.Models.Dto
{
    /*    public class AvailableMachinesDto
        {
            public string BranchName { get; set; } = string.Empty;
            public double Distance { get; set; }
            public int BranchId { get; set; }
            public double Price { get; set; }
            public bool AfterBookingSlot { get; set; }
            public bool BeforeBookingSlot { get; set; }
            public int MachineId { get; set; }
        }*/
    public class BranchesData
    {
        public string BranchName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public int BranchId { get; set; }
        public int MachineId { get; set; }
    }

    public class AvailableMachinesDto
    {
        public List<BranchesData> ReceivedMachines { get; set; } = new List<BranchesData>();
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}

