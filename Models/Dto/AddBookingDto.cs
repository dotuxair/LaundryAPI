using FYP.API.Models.Domain;

namespace FYP.API.Models.Dto
{
    public class AddBookingDto
    {
        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int Cycles { get; set; }

        public int TotalPrice { get; set; }
        public int BranchId { get; set; }
        public int MachineId { get; set; }
        public int ProgramId { get; set; }
        public List<ItemsDto>? Items { get; set; }

    }
    public class ItemsDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
