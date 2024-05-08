using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMSMvcCore.Models
{
    public class IndividualDetails
    {
        public int Id { get; set; }
        public decimal Salary { get; set; }
        [Display(Name = "Rank/Position")]
        public string RankPosition { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        [Display(Name = "IPPS Number")]
        public string IPPSNumber { get; set; }

        /*[Display(Name = "Customer")]
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }*/
    }
}
