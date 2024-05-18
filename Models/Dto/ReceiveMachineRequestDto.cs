namespace FYP.API.Models.Dto
{
    public class ReceivedRequestDto
    {
        public string SelectedOption { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int ProgramId { get; set; }
        public int CapacityId { get; set; }
        public int Cycles { get; set; }
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