using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMSMvcCore.Models
{
    public class CompanyDetails
    {
        public int Id { get; set; }
        [Display(Name = "")]
        public string CACNumber { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Earn Per Month")]
        public decimal EarnPerMonth { get; set; }
        [Display(Name = "Company Address")]
        [DataType(DataType.MultilineText)]
        public string CompanyAddress { get; set; }
        [Display(Name = "Company Phone Number")]
        public long CompanyPhoneNumber { get; set; }
        public long BVN { get; set; }
        public long NIN { get; set; }
        [Display(Name = "Account Number")]
        public long AccountNumber { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        //[Display(Name = "Customer")]
        //public int ApplicationUserId { get; set; }
        //public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
