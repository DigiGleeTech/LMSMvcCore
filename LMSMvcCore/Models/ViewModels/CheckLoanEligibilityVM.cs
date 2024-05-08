namespace LMSMvcCore.Models.ViewModels
{
    public class CheckLoanEligibilityVM
    {
        public decimal Amount { get; set; }
        public decimal Salary { get; set; }
        public decimal AmountPayedPerMonth { get; set; }
        public double InterestRate { get; set; }
        public decimal Reminder { get; set; }
        public decimal ReminderPercent { get; set; }
        public decimal TotalAmountToPay { get; set; }

    }
}
