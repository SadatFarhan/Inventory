namespace Inventory.Models
{
    public class Items
    {
         public int Id { get; set; }
            public string? ItemName { get; set; }
            public string? Description { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Inventory")]
        public int InventoryId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("InventoryId")]
        public Inventorys? Inventory { get; set; }

    }
}
