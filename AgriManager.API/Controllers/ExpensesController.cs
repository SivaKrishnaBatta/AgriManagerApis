using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AgriManager.API.Data;
using AgriManager.API.Models;
using AgriManager.API.DTOs;

namespace AgriManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public ExpensesController(AgriManagerDbContext context)
        {
            _context = context;
        }

        // Get CustomerId from JWT
        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            var expense = new Expense
            {
                CropId = dto.CropId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,

                // DateTime → DateOnly
                ExpenseDate = DateOnly.FromDateTime(dto.ExpenseDate),

                Notes = dto.Notes,

                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            int customerId = GetCustomerId();

            var expenses = await (
                from e in _context.Expenses

                join c in _context.Crops
                    on new { e.CropId, e.CustomerId }
                    equals new { c.CropId, c.CustomerId }

                join cat in _context.ExpenseCategories
                    on new { e.CategoryId, e.CustomerId }
                    equals new { cat.CategoryId, cat.CustomerId }

                where e.CustomerId == customerId

                select new ExpenseListDto
                {
                    ExpenseId = e.ExpenseId,

                    CropId = c.CropId,
                    CropName = c.CropName,

                    CategoryId = cat.CategoryId,
                    CategoryName = cat.CategoryName,

                    Amount = e.Amount,

                    // DateOnly → DateTime
                    ExpenseDate = e.ExpenseDate.ToDateTime(TimeOnly.MinValue),

                    Notes = e.Notes
                }
            ).ToListAsync();

            return Ok(expenses);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            int customerId = GetCustomerId();

            var expense = await (
                from e in _context.Expenses

                join c in _context.Crops
                    on new { e.CropId, e.CustomerId }
                    equals new { c.CropId, c.CustomerId }

                join cat in _context.ExpenseCategories
                    on new { e.CategoryId, e.CustomerId }
                    equals new { cat.CategoryId, cat.CustomerId }

                where e.ExpenseId == id && e.CustomerId == customerId

                select new ExpenseListDto
                {
                    ExpenseId = e.ExpenseId,

                    CropId = c.CropId,
                    CropName = c.CropName,

                    CategoryId = cat.CategoryId,
                    CategoryName = cat.CategoryName,

                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate.ToDateTime(TimeOnly.MinValue),
                    Notes = e.Notes
                }
            ).FirstOrDefaultAsync();

            if (expense == null)
                return NotFound("Expense not found");

            return Ok(expense);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.CustomerId == customerId);

            if (expense == null)
                return NotFound("Expense not found");

            expense.CropId = dto.CropId;
            expense.CategoryId = dto.CategoryId;
            expense.Amount = dto.Amount;

            // DateTime → DateOnly
            expense.ExpenseDate = DateOnly.FromDateTime(dto.ExpenseDate);

            expense.Notes = dto.Notes;

            expense.ModifiedAt = DateTime.Now;
            expense.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            int customerId = GetCustomerId();

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.CustomerId == customerId);

            if (expense == null)
                return NotFound("Expense not found");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok("Expense deleted successfully");
        }
    }
}
