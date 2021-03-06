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
using System.Text.RegularExpressions;

namespace Bangazon.Controllers
{
    public class PaymentTypesController : Controller
    {
        //public static bool IsCreditCardInfoValid(string expiryDate)
        //{
        //    var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
        //    var yearCheck = new Regex(@"^20[0-9]{2}$");


        //    var dateParts = expiryDate.Split('/'); //expiry date in from MM/yyyy            
        //    if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
        //        return false; // ^ check date format is valid as "MM/yyyy"

        //    var year = int.Parse(dateParts[1]);
        //    var month = int.Parse(dateParts[0]);
        //    var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
        //    var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

        //    //check expiry greater than today & within next 6 years <7, 8>>
        //    return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        //}

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public PaymentTypesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: PaymentTypes
        public async Task<IActionResult> Index()
        {
            // Get the current user
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.PaymentType.Include(p => p.User).Where(p => p.User == user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PaymentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            var paymentType = await _context.PaymentType
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentTypeId == id);
            if (paymentType == null)
            {
                return NotFound();
            }

            return View(paymentType);
        }

        // GET: PaymentTypes/Create

         
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: PaymentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentTypeId,DateCreated,Description,AccountNumber,ExpiryDate")] PaymentType paymentType)
        {
            ModelState.Remove("UserId");
            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                paymentType.User = user;
                paymentType.UserId = user.Id;
                _context.Add(paymentType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", paymentType.UserId);
            return View(paymentType);
        }

        // GET: PaymentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentType = await _context.PaymentType.FindAsync(id);
            if (paymentType == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", paymentType.UserId);
            return View(paymentType);
        }

        // POST: PaymentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentTypeId,DateCreated,Description,AccountNumber,UserId")] PaymentType paymentType)
        {
            if (id != paymentType.PaymentTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTypeExists(paymentType.PaymentTypeId))
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
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", paymentType.UserId);
            return View(paymentType);
        }

        // GET: PaymentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            var paymentType = await _context.PaymentType
                .Where(p => p.User == user)
                .FirstOrDefaultAsync(m => m.PaymentTypeId == id);
            if (paymentType == null)
            {
                return NotFound();
            }

            return View(paymentType);
        }

        // POST: PaymentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentType = await _context.PaymentType.FindAsync(id);
            _context.PaymentType.Remove(paymentType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentTypeExists(int id)
        {
            return _context.PaymentType.Any(e => e.PaymentTypeId == id);
        }
    }
}
