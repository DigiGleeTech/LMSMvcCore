using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using LMSMvcCore.Enums;

namespace LMSMvcCore.Models
{
    public class Loan
    {
        public int Id { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Display(Name = "Term Month")]
        public int? TermMonth { get; set; } = 4;
        [Display(Name = "Start Dade")]
        [DataType(DataType.Date)]
        public string? StartDade { get; set; }
        [Display(Name = "End Dade")]
        [DataType(DataType.Date)]
        public string? EndDade { get; set; }
        [Display(Name = "Interest Rate")]
        public double InterestRate { get; set; }
        [Display(Name = "Loan Type")]
        public LoanType? LoanType { get; set; }
        [Required]
        public decimal Salary { get; set; }

        [Display(Name = "Rank/Position")]
        [Required]
        public string RankPosition { get; set; }
        [Required]
        public string Department { get; set; }
        [Display(Name = "PV Number")]
        [Required]
        [StringLength(5, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string PVN { get; set; }
        [AllowNull]
        public string Status { get; set; }  = "PENDING";
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }

    }
}
