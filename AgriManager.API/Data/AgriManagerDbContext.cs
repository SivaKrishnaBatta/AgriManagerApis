using Microsoft.EntityFrameworkCore;
using AgriManager.API.Models;

namespace AgriManager.API.Data
{
    public class AgriManagerDbContext : DbContext
    {
        public AgriManagerDbContext(DbContextOptions<AgriManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farm> Farms { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<CropStatus> CropStatuses { get; set; }

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }

    }

  
}
