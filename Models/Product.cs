using System.ComponentModel.DataAnnotations;

namespace EmailDemo.Models
{
    public class Product
    {
        [Key]
        public int PkProductId { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Navigation property for multiple images
        public ICollection<ProductImage>? ProductImages { get; set; }
    }
}