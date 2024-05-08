using LMSMvcCore.Data;
using LMSMvcCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LMSMvcCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //ViewBag.TotalLoanAmount = db.Loans.Sum(x => x.Amount);
            //ViewBag.Approved = db.Loans.Where(a => a.Status == "APPROVED").Count();
            //ViewBag.Rejected = db.Loans.Where(a => a.Status == "REJECTED").Count();
            //ViewBag.Pending = db.Loans.Where(a => a.Status == "").Count();

            var currentUser = await userManager.GetUserAsync(User);
            //If current user is customer 
            if (User.IsInRole("Customer"))
            {

                if (db.Loans.Any(p => p.ApplicationUserId == currentUser.Id))
                {
                    if (db.Loans.OrderBy(i => i.Id).LastOrDefault(u => u.ApplicationUserId == currentUser.Id).Status == "APPROVED")
                    {

                        //Report Zone 
                        ViewBag.ApprovedLoanRequest = db.Loans.Where(u => u.ApplicationUserId == currentUser.Id).Where(l => l.Status.ToUpper() == "APPROVED").Count();
                        ViewBag.RejectedLoanRequest = db.Loans.Where(u => u.ApplicationUserId == currentUser.Id).Where(l => l.Status.ToUpper() == "REJECTED").Count();
                        ViewBag.PendingLoanRequest = db.Loans.Where(u => u.ApplicationUserId == currentUser.Id).Where(l => l.Status.ToUpper() == "PENDING").Count();
                        ViewBag.CoveredLoanRequest = db.Loans.Where(u => u.ApplicationUserId == currentUser.Id).Where(l => l.Status.ToUpper() == "COVERED").Count();
                        ViewBag.TotalAmountPaid = db.Payments.Where(u => u.ApplicationUserId == currentUser.Id).Sum(a => a.Amount);


                        ViewBag.NextPaymentDateText = "Next Payment Date";
                        ViewBag.AmountPayedText = "Amount Payed";
                        ViewBag.LoanBalanceText = "Loan Balance";
                        ViewBag.LoanStatusText = "Loan Status";

                        ViewBag.LoanStatus = db.Loans.OrderBy(e => e.Id).SingleOrDefault(e => e.ApplicationUserId == currentUser.Id).Status;

                        var amount = db.Payments.Where(e => e.ApplicationUserId == currentUser.Id).ToList();
                        if (amount.Any())
                        {
                            ViewBag.NextPaymentDate = amount.FirstOrDefault().NextDue;
                            ViewBag.AmountPayed = amount.FirstOrDefault().Amount - amount.FirstOrDefault().OutStanding;
                            ViewBag.LoanBalance = amount.FirstOrDefault().OutStanding;
                        }
                        else
                        {
                            ViewBag.NextPaymentDate = "No Active Payment";
                            ViewBag.AmountPayed = "No Active Payment";
                            ViewBag.LoanBalance = "No Active Payment";
                        }

                    }
                    else
                    {
                        ViewBag.NextPaymentDate = "No Any Active Loan";
                        ViewBag.NextPayemtAmount = "No Any Active Loan";
                        ViewBag.TotalAmountRemaining = "No Any Active Loan";
                        ViewBag.LoanStatus = db.Loans.OrderBy(e => e.Id).SingleOrDefault(e => e.ApplicationUserId == currentUser.Id).Status;
                    }
                }
                else
                {
                    ViewBag.NextPaymentDate = "No Any Active Loan";
                    ViewBag.NextPayemtAmount = "No Any Active Loan";
                    ViewBag.TotalAmountRemaining = "No Any Active Loan";
                    ViewBag.LoanStatus = "No Any Active Loan";
                }
            }
            else //else if current logged in user is Admin
            {
                ViewBag.TotalLoanRequest = db.Loans.Where(s => s.Status == "PENDING").Count();
                ViewBag.TotalLoanCovered = db.Loans.Where(s => s.Status == "COVERED").Count();
                ViewBag.TotalLoanApproved = db.Loans.Where(s => s.Status == "APPROVED").Count();
                ViewBag.TotalLoanRejected = db.Loans.Where(s => s.Status == "REJECTED").Count();
                ViewBag.TotalClient = db.applicationUsers.Count(); 
                ViewBag.TotalActiveLoan = db.Loans.Where(s => s.Status == "APPROVED").Count();
                ViewBag.TotalAmountPaid = db.Payments.Sum(a => a.Amount);
                ViewBag.TotalLoanBalance =  Convert.ToDouble(db.Loans.Sum(a => a.Amount)) - db.Payments.Sum(a => a.Amount);


                //Report
                var loanAmount = db.Loans.Where(x => x.Status == "APPROVED").Sum(a => a.Amount);
                var loanInterest = db.Loans.Where(x => x.Status == "APPROVED").Sum(a => a.InterestRate);
                ViewBag.TotalGrantedLoanAmount = loanAmount + Convert.ToDecimal(loanInterest);
                ViewBag.TotalActiveLoan = db.Loans.Where(x => x.Status == "APPROVED").Count();
                ViewBag.TotalCoveredLoan = db.Loans.Where(x => x.Status == "COVERED").Count();
                ViewBag.TotaRejectedlLoan = db.Loans.Where(x => x.Status == "REJECTED").Count();
                ViewBag.TotalPendingLoan = db.Loans.Where(x => x.Status == "PENDING" || x.Status == "").Count();
                ViewBag.TotalProfit = db.Loans.Where(x => x.Status == "APPROVED").Sum(i => i.InterestRate);
                ViewBag.TotalActiveClient = db.applicationUsers.Count();
                ViewBag.TotalUnActiveClient = 0;


            }

            return View();
        }

        public async Task<IActionResult> Report()
        {
            var loanAmount = db.Loans.Where(x => x.Status == "APPROVED").Sum(a => a.Amount);
            var loanInterest = db.Loans.Where(x => x.Status == "APPROVED").Sum(a => a.InterestRate);
            ViewBag.TotalGrantedLoanAmount = loanAmount + Convert.ToDecimal(loanInterest);
            ViewBag.TotalActiveLoan = db.Loans.Where(x => x.Status == "APPROVED").Count();
            ViewBag.TotalCoveredLoan = db.Loans.Where(x => x.Status == "COVERED").Count();
            ViewBag.TotaRejectedlLoan = db.Loans.Where(x => x.Status == "REJECTED").Count();
            ViewBag.TotalPendingLoan = db.Loans.Where(x => x.Status == "PENDING" || x.Status == "").Count();
            ViewBag.TotalProfit = db.Loans.Where(x => x.Status == "APPROVED").Sum(i => i.InterestRate);
            ViewBag.TotalActiveClient = db.applicationUsers.Count();
            ViewBag.TotalUnActiveClient = 0;
          
            return View();
        }

        public IActionResult AllClient()
        {

            return View(db.applicationUsers.ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
