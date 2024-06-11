namespace FYP.API.Models.Dto
{
    public class GetMachinesDto
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Price { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public string loadCapacityName { get; set; } = string.Empty;
    }

}
