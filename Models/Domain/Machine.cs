
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Machine
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Price { get; set; }


        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int LoadCapacityId { get; set; }
        public LoadCapacity? LoadCapacity { get; set; }
    }
}



