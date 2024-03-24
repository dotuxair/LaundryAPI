namespace FYP.API.Models.Domain
{
    public class Retailer
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
