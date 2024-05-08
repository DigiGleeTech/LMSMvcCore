using System;
using System.ComponentModel.DataAnnotations;

namespace LMSMvcCore.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Display(Name = "Amount Payed")]
        public double Amount { get; set; }

        [Display(Name = "Date Payed")]
        [DataType(DataType.Date)]
        public string PaymentDate { get; set; }

        [Display(Name = "Loan")]
        public int? LoanId { get; set; }
        [Display(Name = "Next Due")]
        [DataType(DataType.Date)]
        public string? NextDue { get; set; }
        [Display(Name = "Out-Standing")]
        public double OutStanding { get; set; }
        public Loan? Loan { get; set; }
        [Display(Name = "Customer")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
