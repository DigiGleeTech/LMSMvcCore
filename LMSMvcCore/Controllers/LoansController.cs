using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMSMvcCore.Data;
using LMSMvcCore.Models;
using LMSMvcCore.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LMSMvcCore.Models.ViewModels;
using System.Collections;
using System.Globalization;

namespace LMSMvcCore.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public LoansController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Loans
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User); 
            var loan = _context.Loans
                .Include(a => a.ApplicationUser)
                .Where(x => x.ApplicationUserId == currentUser.Id);
            return View(loan);
        }

        // GET: Loans/ManageLoanRequest
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageLoanRequest()
        {

            var loan = _context.Loans
                .Include(a => a.ApplicationUser)
                .Include(a => a.Payments);
            return View(await loan.ToListAsync());
        }

        // GET: Loans/Details/5
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            DateTime NextDue = Convert.ToDateTime(loan.StartDade).AddMonths(1);
            ViewBag.NextDue = $"{NextDue.Month} {NextDue.Year}";
            ViewBag.TotalAmount = Convert.ToDouble(loan.Amount) + loan.InterestRate;

            return View(loan);
        }

        // GET: Loans/ApproveLoad/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveLoad(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = "APPROVED";
            _context.Update(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Loans/RejectLoan/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectLoan(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = "REJECTED";
            _context.Update(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Loans/CheckElibigility/
        public IActionResult CheckElibigility(string Salary, string Amount, string Interest)
        {
            var salary = Convert.ToDecimal(Salary);
            var amount = Convert.ToDecimal(Amount);
            var interestRate = Convert.ToDecimal(Interest);
            var payableAmount = (amount + interestRate) / 4;
            var reminder = payableAmount % salary;
            var reminderPercent = (reminder / salary) * 100;

            var totalTotalpayment = payableAmount * 4;
            var LoanEligibilityVM = new List<CheckLoanEligibilityVM>
            {
                new CheckLoanEligibilityVM()
                {
                    Amount = amount,
                    Salary = salary,
                    InterestRate = (double)interestRate,
                    AmountPayedPerMonth = payableAmount,
                    Reminder = reminder,
                    ReminderPercent = reminderPercent,
                    TotalAmountToPay  = totalTotalpayment
                }
            };
           
            return Json(LoanEligibilityVM);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public IActionResult ValidatePVNumber(string pvn)
        {
            var pv = _context.Loans.SingleOrDefault(p => p.PVN == pvn.ToString());
            if (pv != null)
            {
                return Json("PV Number Already exist");
            }
            else
            {
                return Ok();
            }
        }

        // GET: Loans/Create
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ApplyloanAsync()
        {

            var currentUser = await userManager.GetUserAsync(User);
            var loanStatus = await _context.Loans
                .FirstOrDefaultAsync(x => x.ApplicationUserId == currentUser.Id);

            if(loanStatus != null)
            {
                return RedirectToAction(nameof(Index));
            }

            var LoanTypeEnum = Enum.GetValues(typeof(LoanType)).Cast<LoanType>();
            var LoanTypeList = LoanTypeEnum.Select(l => new SelectListItem
            {
                Value = l.ToString(),
                Text = l.ToString()
            }).ToList();
            ViewBag.LoanType = LoanTypeList;

            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Applyloan(Loan loan)
        {
            var pv = _context.Loans.SingleOrDefault(p => p.PVN == loan.PVN);
            if (pv != null)
            {
                return View(loan);
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                loan.StartDade = DateTime.Now.Date.ToString("dd/MMM/yyyy");
                loan.EndDade = DateTime.Now.Date.AddMonths(4).ToString("dd/MMM/yyyy");
                loan.TermMonth = 4;
                loan.ApplicationUserId = user.Id;
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var LoanTypeEnum = Enum.GetValues(typeof(LoanType)).Cast<LoanType>();
            var LoanTypeList = LoanTypeEnum.Select(l => new SelectListItem
            {
                Value = l.ToString(),
                Text = l.ToString()
            }).ToList();
            ViewBag.LoanType = LoanTypeList;

            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,TermMonth,StartDade,EndDade,InterestRate,LoanTypeId,LoanType,ApplicationUserId")] Loan loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id))
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
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}
