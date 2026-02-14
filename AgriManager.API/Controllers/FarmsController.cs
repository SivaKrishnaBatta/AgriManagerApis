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
    [Route("api/farms")]
    public class FarmsController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public FarmsController(AgriManagerDbContext context)
        {
            _context = context;
        }

        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ✅ CREATE FARM
        [HttpPost]
        public async Task<IActionResult> CreateFarm(FarmCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var farm = new Farm
            {
                FarmName = dto.FarmName,
                Location = dto.Location,
                TotalFields = dto.TotalFields,
                Notes = dto.Notes,
                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<Farm>
            {
                Status = true,
                Message = "Farm created successfully",
                Data = farm
            });
        }

        // ✅ GET ALL FARMS
        [HttpGet]
        public async Task<IActionResult> GetAllFarms()
        {
            int customerId = GetCustomerId();

            var farms = await _context.Farms
                .Where(f => f.CustomerId == customerId)
                .ToListAsync();

            return Ok(new ApiResponseDto<List<Farm>>
            {
                Status = true,
                Message = "Farms fetched successfully",
                Data = farms
            });
        }

        // ✅ GET FARM BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmById(int id)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Farm not found",
                    Data = null
                });
            }

            return Ok(new ApiResponseDto<Farm>
            {
                Status = true,
                Message = "Farm fetched successfully",
                Data = farm
            });
        }

        // ✅ UPDATE FARM
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, FarmCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Farm not found",
                    Data = null
                });
            }

            farm.FarmName = dto.FarmName;
            farm.Location = dto.Location;
            farm.TotalFields = dto.TotalFields;
            farm.Notes = dto.Notes;
            farm.ModifiedAt = DateTime.Now;
            farm.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<Farm>
            {
                Status = true,
                Message = "Farm updated successfully",
                Data = farm
            });
        }

        // ✅ DELETE FARM
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Farm not found",
                    Data = null
                });
            }

            _context.Farms.Remove(farm);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<object>
            {
                Status = true,
                Message = "Farm deleted successfully",
                Data = null
            });
        }
    }
}
