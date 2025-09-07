using Inventory.Data;
using   Inventory.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class InventoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public InventoryController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // POST: Inventory/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inventorys inventory, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uniqueFileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                inventory.ImageUrl = "/images/" + uniqueFileName;
            }

            _context.Add(inventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(inventory);
    }

    // GET: Inventory/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _context.Inventories
            .Include(i => i.Comment)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (inventory == null)
        {
            return NotFound();
        }

        return View(inventory);
    }

    // POST: Inventory/AddComment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(Comments comment)
    {
        if (ModelState.IsValid)
        {
            comment.UserId = "current_user_id_here"; 
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = comment.InventoryId });
        }
        return RedirectToAction("Details", new { id = comment.InventoryId });
    }
  
    [HttpGet]
    public IActionResult GetItemsPartial(int inventoryId)
    {
        // Fetch the list of items directly
        var items = _context.Items.Where(i => i.InventoryId == inventoryId).ToList();

        return PartialView("_ItemsPartial", items);
    }

    [HttpGet]
    public IActionResult GetChatPartial(int inventoryId)
    {
        // Fetch comments/discussion for the given inventoryId
        var inventory = _context.Inventories.Include(i => i.Comment).FirstOrDefault(i => i.Id == inventoryId);
        return PartialView("_ChatPartial", inventory);
    }
    // Add similar actions for Settings, CustomID, and Fields
    public IActionResult CustomID(int inventoryId)
    {
        var inventory = _context.Inventories.Include(i => i.Comment).FirstOrDefault(i => i.Id == inventoryId);
        return PartialView("_CustomIdPartial", inventory);
    }
}