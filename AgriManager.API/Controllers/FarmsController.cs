using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AgriManager.API.Data;
using AgriManager.API.Models;
using AgriManager.API.DTOs;

namespace AgriManager.API.Controllers
{
    [Authorize] // 🔐 IMPORTANT
    [ApiController]
    [Route("api/farms")]
    public class FarmsController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public FarmsController(AgriManagerDbContext context)
        {
            _context = context;
        }

        // 🔑 Helper: get logged-in CustomerId from JWT
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
                CreatedBy = customerId, // or UserId if you want
                CreatedAt = DateTime.Now
            };

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            return Ok(farm);
        }

        // ✅ READ ALL FARMS (ONLY LOGGED CUSTOMER)
        [HttpGet]
        public async Task<IActionResult> GetAllFarms()
        {
            int customerId = GetCustomerId();

            var farms = await _context.Farms
                .Where(f => f.CustomerId == customerId)
                .ToListAsync();

            return Ok(farms);
        }

        // ✅ READ FARM BY ID (SECURE)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmById(int id)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
                return NotFound("Farm not found");

            return Ok(farm);
        }

        // ✅ UPDATE FARM (ONLY OWN FARM)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, FarmCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
                return NotFound("Farm not found");

            farm.FarmName = dto.FarmName;
            farm.Location = dto.Location;
            farm.TotalFields = dto.TotalFields;
            farm.Notes = dto.Notes;
            farm.ModifiedAt = DateTime.Now;
            farm.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(farm);
        }

        // ✅ DELETE FARM (ONLY OWN FARM)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            int customerId = GetCustomerId();

            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.FarmId == id && f.CustomerId == customerId);

            if (farm == null)
                return NotFound("Farm not found");

            _context.Farms.Remove(farm);
            await _context.SaveChangesAsync();

            return Ok("Farm deleted successfully");
        }
    }
}
