using System.ComponentModel.DataAnnotations;
namespace Inventory.Models
{
    public class Login
    {
       
        public string? NameOrEmail { get; set; }
        public string? Password { get; set; }
       
    }
}
