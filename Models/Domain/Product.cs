
using Microsoft.EntityFrameworkCore;

namespace FYP.API.Models.Domain
{
    
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
 