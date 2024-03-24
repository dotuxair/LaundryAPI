
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class LaundryMachine
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public int LoadCapacity { get; set; }
        public string MachineType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Price { get; set; }


        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public int? RetailerId { get; set; }
        public Retailer? Retailer { get; set; }
    }
}
