using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }


        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }
        public BranchManager? BranchManager { get; set; }

        public BranchManager? BranchManager { get; set; }
    }
}
