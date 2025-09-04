namespace Inventory.Models
{
    public class Comments
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventorys? Inventory { get; set; }
        public string? UserId { get; set; }
        public string? Content { get; set; }
        public string? UserName { get; set; }
        = null;
       
    }
}
