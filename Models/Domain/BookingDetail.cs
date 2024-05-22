using System.ComponentModel.DataAnnotations.Schema;

namespace FYP.API.Models.Domain
{
    public class BookingDetail
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Rounds { get; set; }

        public Booking? Booking { get; set; }
        public int? BookingId { get; set; }


        public int? MachineId { get; set; }
        public Machine? Machine { get; set; }

        public int? LaundryProgramId { get; set; }
        public LaundryProgram? LaundryProgram { get; set; }



    }
}
