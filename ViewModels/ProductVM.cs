using System.ComponentModel.DataAnnotations;

namespace EmailDemo.ViewModels
{
    public class ProductVM
    {
        [Display(Name = "Role Name")]
        public int? ProductId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}