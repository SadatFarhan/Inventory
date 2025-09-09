using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class CustomField
    {
        public int Id { get; set; }

        public int RegistryId { get; set; }
       

        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string? FieldType { get; set; } // e.g., "string", "multiline", "number", "link", "boolean"

        public bool ShowInTableView { get; set; }
    }
}
