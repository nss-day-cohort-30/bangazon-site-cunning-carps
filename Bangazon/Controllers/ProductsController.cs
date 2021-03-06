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
using Microsoft.AspNetCore.Http;
using System.IO;
using Bangazon.Models.ProductViewModels;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _userManager = userManager;
        }
        // GET: Products

        public async Task<IActionResult> Index(string searchterm)
        {
            if (searchterm != null)
            {
                // user can search by product.title or product.city
                var applicationDbContext = _context.Product.Where(p => p.Title.Contains(searchterm) || p.City.Contains(searchterm))
                    .Include(p => p.User)
                    .Include(p => p.ProductType)
                    .OrderByDescending(p => p.DateCreated)
                    .Take(20);
                return View(await applicationDbContext.ToListAsync());
            }

            else
            {
                var applicationDbContext = _context.Product.Include(p => p.User).Include(p => p.ProductType).OrderByDescending(p => p.DateCreated).Take(20);
                return View(await applicationDbContext.ToListAsync());
            }
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        // GET: Products
        [Authorize]
        public async Task<IActionResult> MyProducts()
        {

            var user = await GetCurrentUserAsync();

            var applicationDbContext = _context.Product
                                       .Where(p => p.User == user)
                                       .Include(p => p.ProductType)
                                       .OrderByDescending(p => p.DateCreated);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            UploadImageViewModel viewproduct = new UploadImageViewModel();
            viewproduct.product = new Product();
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View(viewproduct);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadImageViewModel viewproduct)
        {
            ModelState.Remove("product.UserId");
            var user = await GetCurrentUserAsync();

            viewproduct.product.User = user;
            viewproduct.product.UserId = user.Id;

            if (ModelState.IsValid)
            {
                if (viewproduct.ImageFile != null)
                {
                    var fileName = Path.GetFileName(viewproduct.ImageFile.FileName);
                    Path.GetTempFileName();
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewproduct.ImageFile.CopyToAsync(stream);
                    }

                    viewproduct.product.ImagePath = viewproduct.ImageFile.FileName;
                }
                    
                    _context.Add(viewproduct.product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", viewproduct.product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", viewproduct.product.UserId);
            return View(viewproduct.product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,ProductTypeId")] Product product)
        {
            if (id != product.ProductId)
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
                    if (!ProductExists(product.ProductId))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            var user = await GetCurrentUserAsync();
        var order = await _context.Order.FirstOrDefaultAsync
            (o => o.User == user && o.DateCompleted == null);
            
        OrderDetailViewModel shoppingCart = new OrderDetailViewModel();

            if (order == null)
            {
                shoppingCart.Error = "Your shopping cart is empty";
                return View(shoppingCart);
            }                       

        shoppingCart.Order = order;
            shoppingCart.LineItems =
            from op in _context.OrderProduct
            join p in _context.Product
            on op.ProductId equals p.ProductId
            where op.OrderId == shoppingCart.Order.OrderId
            group new { p, op
    }
    by p into productList
            select new OrderLineItem()
    {
        Product = productList.Key,
                Units = productList.Select(p => p.p.ProductId).Count(),
                Cost = productList.Select(p => p.p.ProductId).Count() * productList.Key.Price
            };

            shoppingCart.PaymentTypes = _context.PaymentType.Where(p => p.User == user).Select(p => new SelectListItem
            {
                Text = p.Description,
                Value = p.PaymentTypeId.ToString()
            }).ToList();

            return View(shoppingCart);
}
public async Task<IActionResult> AddToOrder([FromRoute] int id)
        {
            Product productToAdd = await _context.Product.SingleOrDefaultAsync(p => p.ProductId == id);

            // Get the current user
            var user = await GetCurrentUserAsync();

            // See if the user has an open order
            var openOrder = await _context.Order.FirstOrDefaultAsync(o => o.User == user && o.DateCompleted == null);

            Order order = null;

            // If no order, create one, else add to existing order
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
            }

                 order = await _context.Order.FirstOrDefaultAsync(o => o.User == user && o.DateCompleted == null);

                var newOrderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = id
                };

                _context.Add(newOrderProduct);
                _context.SaveChanges();            


            OrderDetailViewModel shoppingCart = new OrderDetailViewModel();
            shoppingCart.Order = order;
            shoppingCart.LineItems =
                from op in _context.OrderProduct
                join p in _context.Product
                on op.ProductId equals p.ProductId
                where op.OrderId == shoppingCart.Order.OrderId
                group new { p, op } by p into productList           
                select new OrderLineItem()
                {
                    Product = productList.Key,
                    Units = productList.Select(p => p.p.ProductId).Count(),
                    Cost = productList.Select(p => p.p.ProductId).Count() * productList.Key.Price
                };

            shoppingCart.PaymentTypes = _context.PaymentType.Where(p => p.User == user).Select(p => new SelectListItem
            {
                Text = p.Description,
                Value = p.PaymentTypeId.ToString()
            }).ToList();
                                                
            return View("ShoppingCart", shoppingCart);
        }

        public async Task<IActionResult> Purchase([FromForm] OrderDetailViewModel Model)
        {
            var user = await GetCurrentUserAsync();

            var order = await _context.Order.SingleOrDefaultAsync(o => o.OrderId == Model.Order.OrderId);

            order.PaymentTypeId = Model.Order.PaymentTypeId;
            order.DateCompleted = DateTime.Now;

            _context.Update(order);
            _context.SaveChanges();

            List<OrderProduct> orderproducts = await _context.OrderProduct.Where(op => op.OrderId == order.OrderId).ToListAsync();
            List<Product> products = await _context.Product.ToListAsync();

            orderproducts.ForEach (op =>
            {
                var product = products.Find(p => p.ProductId == op.ProductId);

                var newProductQuantity = product.Quantity - 1;
                product.Quantity = newProductQuantity;
                               
                _context.Update(product);
                _context.Entry(product).State = EntityState.Modified;
            });

            var shoppingCart = new OrderDetailViewModel();
            bool negativeQuantity = false;

            products.ForEach(p =>
            {
                if (p.Quantity <0)
                {
                    shoppingCart.Error = "Sorry, at least one of the products in your cart is out of stock";
                    negativeQuantity = true;
                }
            });
            if (negativeQuantity == false)
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            } else
            {
                return View("ShoppingCart", shoppingCart);
            }         
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

            private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
