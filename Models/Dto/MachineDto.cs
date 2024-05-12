namespace FYP.API.Models.Dto
{
    public class MachineDto
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public int LoadCapacity { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Price { get; set; }
    }

}
