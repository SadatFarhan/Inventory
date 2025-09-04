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
        public async Task<IActionResult> EditInventory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id && i.CreatedById.ToString() == userId);

            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: User/EditInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInventory(int id, Inventorys inventory)
        {
            if (id != inventory.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (inventory.CreatedById.ToString() != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.Update(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
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