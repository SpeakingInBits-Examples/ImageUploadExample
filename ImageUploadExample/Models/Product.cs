using System.ComponentModel.DataAnnotations;

namespace ImageUploadExample.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// User facing display name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The URL of the product image
    /// </summary>
    public required string ImageUrl { get; set; }
}
