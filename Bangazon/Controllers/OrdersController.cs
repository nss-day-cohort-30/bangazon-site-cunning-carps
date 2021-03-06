﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Bangazon.Models.OrderViewModels;
using System.Data.SqlClient;

namespace Bangazon.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Order.Include(o => o.PaymentType).Include(o => o.User).Where(o => o.User.Id == user.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User == user)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,DateCreated,DateCompleted,UserId,PaymentTypeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        //returns to view with the hyperlinks
        public async Task<IActionResult> Reports()
        {
            return View();
        }

        //gets users that have multiple open orders and then displays first name, last name, and order count
        //otherwise displays the multiple orders not found view
        public async Task<IActionResult> GetOpenOrders()
        {


            var usersWithOpenOrders =  _context.ApplicationUsers
                .Include(u => u.Orders)
                .Where(u => u.Orders.Any(o => o.DateCompleted == null))
                .ToList();

            var usersWithMultipleOrders = usersWithOpenOrders
                 .Where(u => u.Orders.Count >= 2);

            var usersWithOpenOrders2 = usersWithMultipleOrders.Count();

            MultipleOrdersViewModel openorders = new MultipleOrdersViewModel();

            if (usersWithOpenOrders2 == 0)
            {
                openorders.Error = "There are no users with multiple orders";
                return View(openorders);
            } else
            {
                openorders.ApplicationUsers = usersWithMultipleOrders;
                 return View(openorders);
            }
        }


        public async Task<IActionResult> Purchase([FromRoute] int id)
        {

            Product productToAdd = await _context.Product.SingleOrDefaultAsync(p => p.ProductId == id);

            // Get the current user
            var user = await GetCurrentUserAsync();

            // See if the user has an open order
            var openOrder = await _context.Order.SingleOrDefaultAsync(o => o.User == user && o.DateCompleted == null);

            // If no order, create one, else add to existing order
            Order returnedOrder = null;
            if (openOrder == null)
            {
                var newOrder = new Order
                {
                    DateCreated = DateTime.Now,
                    DateCompleted = null,
                    UserId = user.Id,
                    PaymentTypeId = null,
                };

                _context.Add(newOrder);
                _context.SaveChanges();
                returnedOrder = newOrder;

            }
            else

            {
                var order = await _context.Order.SingleOrDefaultAsync(o => o.UserId == user.Id);
                var newOrderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = id
                };
                _context.Add(newOrderProduct);
                _context.SaveChanges();
                returnedOrder = order;
            }

            return View("OrderDetails", returnedOrder);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,DateCreated,DateCompleted,UserId,PaymentTypeId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User == user)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
