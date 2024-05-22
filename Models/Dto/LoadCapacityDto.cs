namespace FYP.API.Models.Dto
{
    public class LoadCapacityDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
