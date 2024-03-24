namespace FYP.API.Models.Dto
{
    public class ReceiveMachineRequestDto
    {
        public List<MachineCapacityDto>? Requirements { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class MachineCapacityDto
    {
        public string Capacity { get; set; } = string.Empty;
    }
}