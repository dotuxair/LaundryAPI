using FYP.API.Models.Domain;

namespace FYP.API.Models.Dto
{

    public class ProgramDto

    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int SpinSpeed { get; set; }
        public string Temprature { get; set; } = string.Empty;
        public int Price { get; set; }
    }


}
