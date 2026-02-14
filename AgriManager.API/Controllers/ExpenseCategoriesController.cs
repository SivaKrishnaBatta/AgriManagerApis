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

        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategoryCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                if (string.IsNullOrWhiteSpace(dto.CategoryName))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Category name is required",
                        Data = null
                    });
                }

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

                return Ok(new ApiResponseDto<ExpenseCategory>
                {
                    Status = true,
                    Message = "Category created successfully",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
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

                return Ok(new ApiResponseDto<List<ExpenseCategoryListDto>>
                {
                    Status = true,
                    Message = "Categories fetched successfully",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
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
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Category not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponseDto<ExpenseCategoryListDto>
                {
                    Status = true,
                    Message = "Category fetched successfully",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ExpenseCategoryCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                var category = await _context.ExpenseCategories
                    .FirstOrDefaultAsync(c => c.CategoryId == id && c.CustomerId == customerId);

                if (category == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Category not found",
                        Data = null
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.CategoryName))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Category name is required",
                        Data = null
                    });
                }

                category.CategoryName = dto.CategoryName;
                category.ModifiedAt = DateTime.Now;
                category.ModifiedBy = customerId;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<ExpenseCategory>
                {
                    Status = true,
                    Message = "Category updated successfully",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= ENABLE / DISABLE =================
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleCategoryStatus(int id)
        {
            try
            {
                int customerId = GetCustomerId();

                var category = await _context.ExpenseCategories
                    .FirstOrDefaultAsync(c => c.CategoryId == id && c.CustomerId == customerId);

                if (category == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Category not found",
                        Data = null
                    });
                }

                category.IsActive = !category.IsActive;
                category.ModifiedAt = DateTime.Now;
                category.ModifiedBy = customerId;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<object>
                {
                    Status = true,
                    Message = "Category status updated successfully",
                    Data = new
                    {
                        category.CategoryId,
                        category.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
