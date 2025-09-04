using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Models;
using Inventory.Data;

namespace Inventory.Models
{
    public class Inventorys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Item Name is required.")]
        [StringLength(100)]
        public string? ItemName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Foreign Key to link to the User who created the inventory
        public int CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Users ? Creator { get; set; }
        public string? ImageUrl { get; internal set; }
        public ICollection<Comments>? Comment { get; set; }
        public ICollection<Items>? Items { get; set; }
        public int CreatorId { get; internal set; }
    }
}
