using LMSMvcCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMSMvcCore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<IndividualDetails> Individuals { get; set; }
        public DbSet<CompanyDetails> Companies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FoodStuff> FoodStuffs { get; set; }
    }
}
