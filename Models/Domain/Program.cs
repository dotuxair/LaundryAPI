
namespace FYP.API.Models.Domain
{
    public class LaundryProgram
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Temprature { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string SpinSpeed { get; set; } = string.Empty;
        public int Price { get; set; }

        public int? AdminId { get; set; }
        public Admin?   Admin { get; set; }
    }
}
