namespace ImageUploadExample.ViewModels
{
    public class ProductCreateViewModel
    {
        public required string Name { get; set; }

        public required IFormFile ProductImage { get; set; }
    }
}
