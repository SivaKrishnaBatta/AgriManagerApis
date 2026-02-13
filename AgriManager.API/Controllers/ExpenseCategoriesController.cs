using AgriManager.API.Data;
using AgriManager.API.DTO;
using AgriManager.API.DTOs;
using AgriManager.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expense-categories")]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public ExpenseCategoriesController(AgriManagerDbContext context)
        {
            _context = context;
        }

        // 🔑 Get CustomerId from JWT
        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategoryCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var category = new ExpenseCategory
            {
                CategoryName = dto.CategoryName,
                IsActive = true,

                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            int customerId = GetCustomerId();

            var categories = await _context.ExpenseCategories
                .Where(c => c.CustomerId == customerId)
                .Select(c => new ExpenseCategoryListDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive
                })
                .OrderByDescending(c => c.IsActive)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();

            return Ok(categories);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            int customerId = GetCustomerId();

            var category = await _context.ExpenseCategories
                .Where(c => c.CategoryId == id && c.CustomerId == customerId)
                .Select(c => new ExpenseCategoryListDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ExpenseCategoryCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var category = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.CustomerId == customerId);

            if (category == null)
                return NotFound("Category not found");

            category.CategoryName = dto.CategoryName;
            category.ModifiedAt = DateTime.Now;
            category.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // ================= ENABLE / DISABLE =================
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleCategoryStatus(int id)
        {
            int customerId = GetCustomerId();

            var category = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.CustomerId == customerId);

            if (category == null)
                return NotFound("Category not found");

            category.IsActive = !category.IsActive;
            category.ModifiedAt = DateTime.Now;
            category.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                category.CategoryId,
                category.IsActive
            });
        }
    }
}
