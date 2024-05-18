namespace FYP.API.Models.Dto
{
    public class GetMachinesDto
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public int LoadCapacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    public class LoadCapacityDto
    {
        public int Id { get; set; }
        public int LoadCapacity { get; set; }
        public string LoadCapacityDescription { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}
