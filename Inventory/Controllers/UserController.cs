using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inventory.Controllers
{
    [Authorize] 
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allInventories = await _context.Inventories.Include(i => i.Creator).ToListAsync();
            return View(allInventories); // The controller is returning a list
        }
        // GET: User/CreateInventory
        public IActionResult CreateInventory()
        {
            return View();
        }

        // POST: User/CreateInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInventory(Inventorys inventory)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                inventory.CreatedById = int.Parse(userId);
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: User/EditInventory
        [Authorize] // Requires the user to be logged in
        public async Task<IActionResult> EditInventory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(Index); // User is authenticated but somehow has no ID
            }

            // Fetch the inventory and  check ownership
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == id && i.CreatedById.ToString() == userId);

            if (inventory == null)
            {
                return NotFound(); // Either not found or not owned by this user
            }

            return View(inventory);
        }

        // POST: Inventory/EditInventory/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Requires the user to be logged in
        public async Task<IActionResult> EditInventory(int id, Inventorys inventory)
        {
            if (id != inventory.Id)
            {
                return NotFound();
            }

            // Fetch the original inventory from the database
            var originalInventory = await _context.Inventories.FindAsync(id);
            if (originalInventory == null)
            {
                return NotFound();
            }

            // Verify that the current user is the owner of this inventory
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (originalInventory.CreatedById.ToString() != userId)
            {
                return Forbid(); // Return a 403 Forbidden status
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manually update the properties on the original object
                    originalInventory.ItemName = inventory.ItemName;
                    originalInventory.Quantity = inventory.Quantity;
                    originalInventory.Description = inventory.Description;
                    originalInventory.ImageUrl = inventory.ImageUrl;

                    // Save changes to the original entity
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)); 
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
            }
            // If ModelState is invalid, return the view with the provided model
            return View(inventory);
        }

        // Helper method to check if an inventory item exists
        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.Id == id);
        }

        // View all inventories for all users
        [AllowAnonymous] // Everyone can view this page, including non-logged-in users
        public async Task<IActionResult> ViewAllInventories()
        {
            var allInventories = await _context.Inventories.Include(i => i.Creator).ToListAsync();
            return View(allInventories);
        }
    }
}