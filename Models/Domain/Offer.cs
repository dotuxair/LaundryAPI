using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int  Percentage { get; set; }
        public int PriceLimit { get; set; }
        public string Status { get; set; } = string.Empty;

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public int? ProgramId { get; set; }
        public Program? Program { get; set; }

    }
}
