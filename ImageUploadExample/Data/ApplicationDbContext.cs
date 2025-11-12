using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageUploadExample.Models;

namespace ImageUploadExample.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<ImageUploadExample.Models.Product> Product { get; set; } = default!;
    }
}
