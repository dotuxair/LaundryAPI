namespace FYP.API.Models.Dto
{
    public class GetMachinesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
