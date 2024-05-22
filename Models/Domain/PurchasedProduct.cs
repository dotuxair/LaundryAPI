using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class PurchasedProduct
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
