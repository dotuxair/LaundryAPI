namespace FYP.API.Models.Dto
{
    public class ReceivedRequestDto
    {
        public string SelectedOption { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int ProgramIdOne { get; set; }
        public int CapacityIdOne { get; set; }
        public int ProgramIdTwo { get; set; }
        public int CapacityIdTwo { get; set; }
        public int ProgramIdThree { get; set; }
        public int CapacityIdThree { get; set; }
        public int MachinesNeeded { get; set; }

        public int LaundryIntervals { get; set; }
        public int Distance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<SelectedItemsList>? Items { get; set; }
    }

    public class SelectedItemsList
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}