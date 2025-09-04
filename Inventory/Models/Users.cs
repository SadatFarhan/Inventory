using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "UserName cannot be longer than 100 characters.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        // This property will store the user's role: "Admin" or "User"
        public string? Role { get; set; } = "User";
       
    }
}
