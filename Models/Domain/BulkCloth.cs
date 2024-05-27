namespace FYP.API.Models.Domain
{
    public class BulkCloth
    {
        public int Id { get; set; }
        public string RequestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal PriceOffered { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? DateAccepted { get; set; }
        public DateTime? DateCompleted { get; set; }

        public DateTime? PickUpDate { get; set; }
        public bool InformUser { get; set; }
        public bool InformManager { get; set; }


        public decimal? AcceptedPrice { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User? User { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
