namespace FYP.API.Models.Dto
{
    public class BulkRequestBranches
    {

        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
