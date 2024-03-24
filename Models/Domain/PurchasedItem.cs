using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class PurchasedItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int? LaundryItemId { get; set; }
        public LaundryItem? LaundryItem { get; set; }

    }
}
