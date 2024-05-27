﻿namespace FYP.API.Models.Dto
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class BranchData
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}
