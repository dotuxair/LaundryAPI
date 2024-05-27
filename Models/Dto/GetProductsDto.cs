namespace FYP.API.Models.Dto
{
    public class GetProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string ProductImageUrl { get; set; } = string.Empty;
    }
}
