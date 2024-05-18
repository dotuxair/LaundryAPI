using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class BookingDetail
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public int Cycles { get; set; }

        public Booking? Booking { get; set; }
        public int? BookingId { get; set; }


        public int? LaundryMachineId { get; set; }
        public LaundryMachine? LaundryMachine { get; set; }



    }
}
