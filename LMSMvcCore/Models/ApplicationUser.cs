using LMSMvcCore.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LMSMvcCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string POB { get; set; }
        public DateTime DOB { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber2 { get; set; }

        [Display(Name = "Account Number")]
        public long AccountNumber { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        /*[AllowNull]
        public virtual ICollection<CompanyDetails> Companies { get; set; }
        [AllowNull]
        public virtual ICollection<IndividualDetails> Individuals { get; set; }*/
        [AllowNull]
        public virtual ICollection<Loan> Loans { get; set; }
        [AllowNull]
        public virtual ICollection<Payment> payments { get; set; }

    }
}
