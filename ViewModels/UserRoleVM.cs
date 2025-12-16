using System.ComponentModel.DataAnnotations;

namespace EmailDemo.ViewModels
{
    public class UserRoleVM
    {
        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
