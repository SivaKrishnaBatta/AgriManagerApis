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
    [Route("api/income")]
    public class IncomeController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public IncomeController(AgriManagerDbContext context)
        {
            _context = context;
        }

        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateIncome([FromBody] IncomeCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                var income = new Income
                {
                    CropId = dto.CropId,
                    Quantity = dto.Quantity,
                    PricePerUnit = dto.PricePerUnit,
                    TotalAmount = dto.TotalAmount,  // TAKE FROM FRONTEND
                    SaleDate = DateOnly.FromDateTime(dto.SaleDate),
                    Notes = dto.Notes,
                    CustomerId = customerId,
                    CreatedBy = customerId,
                    CreatedAt = DateTime.Now
                };

                _context.Incomes.Add(income);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<Income>
                {
                    Status = true,
                    Message = "Income created successfully",
                    Data = income
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
        public async Task<IActionResult> GetAllIncome()
        {
            try
            {
                int customerId = GetCustomerId();

                var incomeList = await (
                    from i in _context.Incomes
                    join c in _context.Crops
                        on new { i.CropId, i.CustomerId }
                        equals new { c.CropId, c.CustomerId }
                    where i.CustomerId == customerId
                    select new IncomeListDto
                    {
                        IncomeId = i.IncomeId,
                        CropId = c.CropId,
                        CropName = c.CropName,
                        Quantity = i.Quantity,
                        PricePerUnit = i.PricePerUnit,
                        TotalAmount = i.TotalAmount,
                        SaleDate = i.SaleDate.ToDateTime(TimeOnly.MinValue),
                        Notes = i.Notes
                    }
                ).ToListAsync();

                return Ok(new ApiResponseDto<List<IncomeListDto>>
                {
                    Status = true,
                    Message = "Income fetched successfully",
                    Data = incomeList
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
        public async Task<IActionResult> GetIncomeById(int id)
        {
            try
            {
                int customerId = GetCustomerId();

                var income = await (
                    from i in _context.Incomes
                    join c in _context.Crops
                        on new { i.CropId, i.CustomerId }
                        equals new { c.CropId, c.CustomerId }
                    where i.IncomeId == id && i.CustomerId == customerId
                    select new IncomeListDto
                    {
                        IncomeId = i.IncomeId,
                        CropId = c.CropId,
                        CropName = c.CropName,
                        Quantity = i.Quantity,
                        PricePerUnit = i.PricePerUnit,
                        TotalAmount = i.TotalAmount,
                        SaleDate = i.SaleDate.ToDateTime(TimeOnly.MinValue),
                        Notes = i.Notes
                    }
                ).FirstOrDefaultAsync();

                if (income == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Income not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponseDto<IncomeListDto>
                {
                    Status = true,
                    Message = "Income fetched successfully",
                    Data = income
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
        public async Task<IActionResult> UpdateIncome(int id, [FromBody] IncomeCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                var income = await _context.Incomes
                    .FirstOrDefaultAsync(i => i.IncomeId == id && i.CustomerId == customerId);

                if (income == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Income not found",
                        Data = null
                    });
                }

                income.CropId = dto.CropId;
                income.Quantity = dto.Quantity;
                income.PricePerUnit = dto.PricePerUnit;
                income.TotalAmount = (dto.Quantity ?? 0) * (dto.PricePerUnit ?? 0);
                income.SaleDate = DateOnly.FromDateTime(dto.SaleDate);
                income.Notes = dto.Notes;
                income.ModifiedAt = DateTime.Now;
                income.ModifiedBy = customerId;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<Income>
                {
                    Status = true,
                    Message = "Income updated successfully",
                    Data = income
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

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            try
            {
                int customerId = GetCustomerId();

                var income = await _context.Incomes
                    .FirstOrDefaultAsync(i => i.IncomeId == id && i.CustomerId == customerId);

                if (income == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Income not found",
                        Data = null
                    });
                }

                _context.Incomes.Remove(income);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<object>
                {
                    Status = true,
                    Message = "Income deleted successfully",
                    Data = null
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
