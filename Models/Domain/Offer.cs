using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int  OffPercentage { get; set; }
        public string Status { get; set; } = string.Empty;

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public int? LaundryProgramId { get; set; }
        public LaundryProgram? LaundryProgram { get; set; }

    }
}
