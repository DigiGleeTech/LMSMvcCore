using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMSMvcCore.Data;
using LMSMvcCore.Models;
using Microsoft.AspNetCore.Identity;

namespace LMSMvcCore.Controllers
{
    public class PayPointsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public PayPointsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: PayPoints
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var payment = _context.Payments
                .Include(p => p.Loan)
                .Where(x => x.ApplicationUserId == currentUser.Id);
            return View(payment);
        }

        // GET: PayPoints/Payment
        public async Task<IActionResult> Payment()
        {
            var payment = _context.Payments
                .Where(a => a.Amount > 0)
                .Include(p => p.Loan)
                .Include(p => p.ApplicationUser);
            return View(payment);
        }

        // GET: PayPoints/MakePayment
        public async Task<IActionResult> MakePayment()
        {

            var payment = await _context.Loans
                .Include(p => p.Payments)
                .Where(a => a.Amount > 0)
                .Include(p => p.ApplicationUser)
                .ToListAsync();

            return View(payment);
        }

        // GET: PayPoints/MakePayment
        [HttpPost]
        public async Task<IActionResult> MakePayment(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.applicationUsers
                .Include(p => p.Loans)
                .Include(p => p.payments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var getLoan = _context.Loans.FirstOrDefault(u => u.ApplicationUserId == user.Id && u.Status == "APPROVED");
            var getpayment = _context.Payments.FirstOrDefault(u => u.ApplicationUserId == user.Id && u.LoanId == getLoan.Id);
            var amount = (getLoan.Amount + Convert.ToDecimal(getLoan.InterestRate)) / 4;
            var outStanding = 0.0;
            if (getpayment != null)
                outStanding = Convert.ToDouble(getpayment.OutStanding) - Convert.ToDouble(amount);
            else
                outStanding = ((double)getLoan.Amount + (double)getLoan.InterestRate) - Convert.ToDouble(amount);
           /* var amount = (Convert.ToDouble(user.Loans.Where(u => u.ApplicationUserId == user.Id).Select(a => a.Amount)) +
                            Convert.ToDouble(user.Loans.Where(u => u.ApplicationUserId == user.Id).Select(a => a.InterestRate)) / 4);
            var outStanding = amount - Convert.ToDouble(user.Loans.Where(u => u.ApplicationUserId == user.Id).Select(a => a.Amount);
            var nextDue = "";*/
            Payment payment = new Payment()
            {
                 Amount = (double)amount,
                 PaymentDate = DateTime.Now.Date.ToString("dddd dd MMM, yyyy"),
                 LoanId = getLoan.Id,
                 ApplicationUserId = getLoan.ApplicationUserId,
                 NextDue = DateTime.Now.Date.AddMonths(1).ToString("MMM, yyyy"),
                 OutStanding = outStanding
            };
            //payment.OutStanding = Convert.ToDouble(outStanding) - payment.Amount;
           
          
            _context.Add(payment);
            var a = await _context.SaveChangesAsync();
            if (a > 0)
            {
                if (Convert.ToDecimal(payment.OutStanding) == getLoan.Amount)
                {
                    getLoan.Status = "COVERED";
                    getLoan.Amount = 0;
                    getLoan.InterestRate = 0;
                    getLoan.TermMonth = 0;
                    _context.Add(getLoan);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    getLoan.TermMonth -= 1;
                    _context.Loans.Add(getLoan);
                }                
              
                await _context.SaveChangesAsync();
                return RedirectToAction("Payment");

            }
            else
            {
                return Ok("Payment Failed");
            }

            //return View(payment);
        }
        

        // GET: PayPoints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Loan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: PayPoints/Create
        public IActionResult Create()
        {
            ViewData["LoanId"] = new SelectList(_context.Loans, "Id", "Department");
            return View();
        }

        // POST: PayPoints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,PaymentDate,LoanId,NextDue,OutStanding,ApplicationUserId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoanId"] = new SelectList(_context.Loans, "Id", "Department", payment.LoanId);
            return View(payment);
        }

        // GET: PayPoints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["LoanId"] = new SelectList(_context.Loans, "Id", "Department", payment.LoanId);
            return View(payment);
        }

        // POST: PayPoints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,PaymentDate,LoanId,NextDue,OutStanding,ApplicationUserId")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            ViewData["LoanId"] = new SelectList(_context.Loans, "Id", "Department", payment.LoanId);
            return View(payment);
        }

        // GET: PayPoints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Loan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: PayPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
