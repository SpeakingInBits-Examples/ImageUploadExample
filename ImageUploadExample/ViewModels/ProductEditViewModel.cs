namespace ImageUploadExample.ViewModels
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }

        public required string CurrentImageUrl { get; set; }

        // Optional - only if user wants to upload a new image
        public IFormFile? ProductImage { get; set; }
    }
}
