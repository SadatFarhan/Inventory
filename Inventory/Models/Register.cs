using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Register
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; } 
    }
}
