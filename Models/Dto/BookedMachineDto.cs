namespace FYP.API.Models.Dto
{
    public class BookedMachineDto
    {
        public int BookingDetailId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public double Price { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string LoadCapacityName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
    }

}
