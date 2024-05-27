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
    }

    public class AvailableMachines
    {
        public int MachineId { get; set; }
        public int ProgramId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BranchId { get; set; }

    }

    public class AvailableMachinesDto
    {
        public List<BranchesData> BranchesList { get; set; } = new List<BranchesData>();
        public List<AvailableMachines> MachinesList { get; set; } = new List<AvailableMachines>();
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
    }

}

