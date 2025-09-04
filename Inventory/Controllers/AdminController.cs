using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Inventory.Controllers
{
    [Authorize(Roles = "Admin")] // শুধুমাত্র "Admin" রোল এই কন্ট্রোলার অ্যাক্সেস করতে পারবে
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard: View all users and inventories 
        public async Task<IActionResult> Index()
        {
            var adminDashboard = new AdminDashboard
            {
                Users = await _context.Users.ToListAsync(),
                InventoryItems = await _context.Inventories.Include(i => i.Creator).ToListAsync()
            };

            return View(adminDashboard);
        }


        // GET: Admin/ChangeRole
        public async Task<IActionResult> ChangeRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Role = role;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // In Inventory.Controllers/AdminController.cs

        // ... (existing code)

        // GET: Admin/CreateInventory
        public IActionResult CreateInventory()
        {
            return View();
        }

        // POST: Admin/CreateInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInventory([Bind("ItemName,Category,Description,Quantity")] Inventorys inventory)
        {
            // Check if the model state is valid and if the user is authenticated
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                // Get the current user's ID from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Find the user object in the database
                var creator = await _context.Users.FindAsync(int.Parse(userId));

                if (creator == null)
                {
                    return Unauthorized(); // Or handle the error appropriately
                }

                // Assign the Creator and CreatorId to the inventory item
                inventory.Creator = creator;
                inventory.CreatorId = creator.Id;

                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect back to the Admin Dashboard
            }
            return View(inventory);
        }

        // ... (existing code)




        // Admin can manage all inventories
        public async Task<IActionResult> ManageInventories()
        {
            var inventories = await _context.Inventories.Include(i => i.Creator).ToListAsync();
            return View(inventories);
        }

        // GET: Admin/EditInventory
        public async Task<IActionResult> EditInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Admin/EditInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInventory(int id, Inventorys inventory)
        {
            if (id != inventory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageInventories));
            }
            return View(inventory);
        }

        // GET: Admin/DeleteInventory
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Admin/DeleteInventory
        [HttpPost, ActionName("DeleteInventory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageInventories));
        }
    }
}