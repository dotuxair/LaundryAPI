
using Microsoft.EntityFrameworkCore;

namespace FYP.API.Models.Domain
{
    
    public class LaundryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        public int? RetailerId { get; set; }
        public Retailer? Retailer { get; set; }


    }
}
 