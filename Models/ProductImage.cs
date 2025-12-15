using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailDemo.Models
{
    public class ProductImage
    {
        [Key]
        public int PkProductImageId { get; set; }

        // Nullable Foreign Key to Product
        [ForeignKey("Product")]
        public int? FkProductId { get; set; }

        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? ImageData { get; set; }

        // Navigation property back to Product (optional)
        public Product? Product { get; set; }
    }
}

