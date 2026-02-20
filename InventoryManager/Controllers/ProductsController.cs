using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Models;
using Microsoft.AspNetCore.Authorization;
using InventoryManager.Constants;

namespace InventoryManager.Controllers
{
    [Authorize]  // Must be logged in to access any action
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? categoryId, string sortOrder)
        {
            // Pass current filter values back to the view so they persist
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSort"] = sortOrder;

            // Sort toggle logic — if you click the same column twice, it reverses
            ViewData["NameSort"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["QtySort"] = sortOrder == "qty_asc" ? "qty_desc" : "qty_asc";

            // Start with all products, including related Category and Supplier
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            // Filter by search text
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description!.Contains(searchString));
            }

            // Filter by category dropdown
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Sort based on column clicked
            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "name_asc" => products.OrderBy(p => p.Name),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "price_asc" => products.OrderBy(p => p.Price),
                "qty_desc" => products.OrderByDescending(p => p.Quantity),
                "qty_asc" => products.OrderBy(p => p.Quantity),
                _ => products.OrderBy(p => p.Name) // default sort
            };

            // Pass categories list for the filter dropdown
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Quantity,LowStockThreshold,DateAdded,CategoryId,SupplierId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "CompanyName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Quantity,LowStockThreshold,DateAdded,CategoryId,SupplierId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
