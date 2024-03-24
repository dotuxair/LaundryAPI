namespace FYP.API.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Qauntity { get; set; }
        public int Price { get; set; }
        public IFormFile? ProductImage { get; set; }

    }
}
