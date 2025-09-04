namespace Inventory.Models
{
    public class AdminDashboard
    {
        public List<Users> Users { get; set; } 
        public List<Inventorys> InventoryItems { get; set; } 

        public List<Comments> Comments { get; set; } 
        public AdminDashboard() { 
        
        }
    }
}
