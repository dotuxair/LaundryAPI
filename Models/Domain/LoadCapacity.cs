namespace FYP.API.Models.Domain
{
    public class LoadCapacity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Price { get; set; }

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

    }
}
