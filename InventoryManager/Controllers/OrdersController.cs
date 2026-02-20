using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Models;

namespace InventoryManager.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Create — anyone logged in can place an order
        public async Task<IActionResult> Create()
        {
            // Load all products that are in stock
            var products = await _context.Products
                .Where(p => p.Quantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var viewModel = new CreateOrderViewModel
            {
                Items = products.Select(p => new OrderItemViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    AvailableStock = p.Quantity,
                    Quantity = 0,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            // Get only selected items with quantity > 0
            var selectedItems = viewModel.Items
                .Where(i => i.IsSelected && i.Quantity > 0)
                .ToList();

            if (!selectedItems.Any())
            {
                ModelState.AddModelError("", "Please select at least one product.");

                // Reload product info since the form doesn't post everything back
                var products = await _context.Products
                    .Where(p => p.Quantity > 0)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                viewModel.Items = products.Select(p => new OrderItemViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    AvailableStock = p.Quantity,
                    Quantity = 0,
                    IsSelected = false
                }).ToList();

                return View(viewModel);
            }

            // Validate stock availability
            foreach (var item in selectedItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {item.ProductName}. Available: {product?.Quantity ?? 0}");

                    var products = await _context.Products
                        .Where(p => p.Quantity > 0)
                        .OrderBy(p => p.Name)
                        .ToListAsync();

                    viewModel.Items = products.Select(p => new OrderItemViewModel
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        Price = p.Price,
                        AvailableStock = p.Quantity,
                        Quantity = 0,
                        IsSelected = false
                    }).ToList();

                    return View(viewModel);
                }
            }

            // Create the order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                Notes = viewModel.Notes,
                OrderItems = new List<OrderItem>()
            };

            // Add items and deduct stock
            foreach (var item in selectedItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product!.Price  // Lock in the price at time of order
                });

                // Deduct from inventory
                product.Quantity -= item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        // GET: Orders/Edit/5 — admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(OrderStatus)), order.Status);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,Status,Notes")] Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(OrderStatus)), order.Status);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Restore stock when deleting an order
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                    }
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}