using System.ComponentModel.DataAnnotations;

namespace EmailDemo.ViewModels
{
    public class UserVM
    {
        [Required]
        public string? Email { get; set; }
    }
}
