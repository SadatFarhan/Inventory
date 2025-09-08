using Inventory.Data;
using Inventory.Migrations;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Inventory.Controllers
{
    [Authorize(Roles = "Admin")] 
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



        // GET: Admin/ChangeRole
        public async Task<IActionResult> EditUser(int id)
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
        public async Task<IActionResult> EditUser(int id, string role , string userName , string email)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = userName;
            user.Email = email;
            user.Role = role;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/DeleteUser/{id}
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/DeleteUser/{id}
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }



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

        // Inside InventoryController.cs

        // GET: Inventory/Edit/{id}
       
        // GET: Admin/EditInventory/{id}
        public async Task<IActionResult> EditInventory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Admin/EditInventory/{id}
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
                try
                {
                    // This is a safer way to update the entity.
                    var inventoryToUpdate = await _context.Inventories.FindAsync(id);
                    if (inventoryToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update only the properties that are editable
                    inventoryToUpdate.ItemName = inventory.ItemName;
                    inventoryToUpdate.Quantity = inventory.Quantity;
                    inventoryToUpdate.Description = inventory.Description;
                    inventoryToUpdate.ImageUrl = inventory.ImageUrl;

                    _context.Update(inventoryToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Redirect back to the admin inventory management page
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }


        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.Id == id);
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
        // The new action to handle multiple user deletions
        [HttpPost]
        
        public async Task<IActionResult> DeleteUsers(string selectedUserIds)
        {
            if (string.IsNullOrEmpty(selectedUserIds))
            {
                return RedirectToAction(nameof(Index));
            }

            var ids = selectedUserIds.Split(',').Select(int.Parse).ToList();

            var usersToDelete = await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();

            if (usersToDelete != null && usersToDelete.Any())
            {
                _context.Users.RemoveRange(usersToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}